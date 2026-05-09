using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Data;
using RT_Inventory.Api.DTOs.IssueSessions;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;
using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Services;

public class IssueSessionService(ApplicationDbContext dbContext) : IIssueSessionService
{
    private static readonly TimeSpan InactivityTimeout = TimeSpan.FromMinutes(15);

    public async Task<(IssueSessionResponseDto? Session, string? Error, int StatusCode)> StartAsync(StartIssueSessionRequestDto request, int startedByUserId)
    {
        var pickingListNo = request.PickingListNo?.Trim();
        var pickingList = request.PickingListId is > 0
            ? await dbContext.PickingLists.FirstOrDefaultAsync(x => x.Id == request.PickingListId.Value)
            : await dbContext.PickingLists.FirstOrDefaultAsync(x => x.PickingListNo == pickingListNo || x.BarcodeValue == pickingListNo);

        if (pickingList is null)
        {
            await AddWarningAsync(WarningType.PickingListNotScanned, "Picking list was not found during issue session start.", null, null, null, null);
            return (null, "Picking list was not found.", StatusCodes.Status404NotFound);
        }

        var session = new IssueSession
        {
            PickingListId = pickingList.Id,
            Status = GateSessionStatus.Active,
            StartedByUserId = startedByUserId,
            StartedAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow
        };

        dbContext.IssueSessions.Add(session);
        await dbContext.SaveChangesAsync();

        var loaded = await LoadSessionAsync(session.Id);
        return (ToSessionResponse(loaded ?? session), null, StatusCodes.Status201Created);
    }

    public async Task<(IssueTagScanResponseDto? Scan, string? Error, int StatusCode)> ScanAsync(int sessionId, IssueTagScanRequestDto request, int performedByUserId)
    {
        var session = await dbContext.IssueSessions
            .Include(x => x.PickingList).ThenInclude(x => x.Requisition).ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == sessionId);

        if (session is null)
        {
            return (null, "Issue session was not found.", StatusCodes.Status404NotFound);
        }

        if (session.Status != GateSessionStatus.Active || DateTime.UtcNow - session.LastActivityAt > InactivityTimeout)
        {
            session.Status = GateSessionStatus.Expired;
            session.EndedAt ??= DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return (null, "Issue session is not active or has expired.", StatusCodes.Status400BadRequest);
        }

        session.LastActivityAt = DateTime.UtcNow;
        var tagValue = request.TagValue.Trim();
        var tag = await dbContext.RfidTags.FirstOrDefaultAsync(x => x.TagValue == tagValue && x.IsActive);
        if (tag is null)
        {
            await AddWarningAsync(WarningType.WrongBagScanned, "RFID tag is not active or was not found.", tagValue, null, session.Id, session.PickingList.RequisitionId);
            return await WarningResponseAsync(session.Id, tagValue, WarningType.WrongBagScanned, "RFID tag is not active or was not found.");
        }

        var stock = await dbContext.Stocks.FirstOrDefaultAsync(x => x.RfidTagId == tag.Id);
        if (stock is null || stock.Status != StockStatus.Confirmed)
        {
            await AddWarningAsync(WarningType.ExtraBagDetected, "RFID tag does not have Confirmed Stock.", tagValue, null, session.Id, session.PickingList.RequisitionId);
            return await WarningResponseAsync(session.Id, tagValue, WarningType.ExtraBagDetected, "RFID tag does not have Confirmed Stock.", tag.Id);
        }

        var matchesRequisition = session.PickingList.Requisition.Items.Any(x =>
            string.Equals(x.ItemYarnType, stock.ItemYarnType, StringComparison.OrdinalIgnoreCase)
            && (string.IsNullOrWhiteSpace(x.LotNo) || string.Equals(x.LotNo, stock.LotNo, StringComparison.OrdinalIgnoreCase)));

        if (!matchesRequisition)
        {
            await AddWarningAsync(WarningType.WrongBagScanned, "RFID tag stock does not match picking list requirements.", tagValue, null, session.Id, session.PickingList.RequisitionId);
            return await WarningResponseAsync(session.Id, tagValue, WarningType.WrongBagScanned, "RFID tag stock does not match picking list requirements.", tag.Id);
        }

        stock.Status = StockStatus.Issued;
        stock.UpdatedAt = DateTime.UtcNow;
        session.PickingList.Requisition.Status = RequisitionStatus.Issued;
        session.PickingList.Requisition.UpdatedAt = DateTime.UtcNow;

        dbContext.StockTransactions.Add(new StockTransaction
        {
            RfidTagId = tag.Id,
            AsnGrnId = stock.AsnGrnId,
            RequisitionId = session.PickingList.RequisitionId,
            TransactionType = StockTransactionType.Issue,
            FromStatus = StockStatus.Confirmed,
            ToStatus = StockStatus.Issued,
            Weight = stock.Weight,
            CreatedByUserId = performedByUserId
        });

        await dbContext.SaveChangesAsync();

        return (new IssueTagScanResponseDto
        {
            IsValid = true,
            Message = "RFID tag issued and stock moved to Issued Stock.",
            IssueSessionId = session.Id,
            TagValue = tagValue,
            RfidTagId = tag.Id,
            StockId = stock.Id
        }, null, StatusCodes.Status200OK);
    }

    public async Task<(IssueSessionResponseDto? Session, string? Error, int StatusCode)> DeactivateAsync(int sessionId)
    {
        var session = await LoadSessionAsync(sessionId);
        if (session is null)
        {
            return (null, "Issue session was not found.", StatusCodes.Status404NotFound);
        }

        session.Status = GateSessionStatus.Deactivated;
        session.EndedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return (ToSessionResponse(session), null, StatusCodes.Status200OK);
    }

    private async Task AddWarningAsync(WarningType type, string message, string? tagValue, int? asnGrnId, int? issueSessionId, int? requisitionId)
    {
        dbContext.WarningLogs.Add(new WarningLog
        {
            WarningType = type,
            Message = message,
            RfidTagValue = tagValue,
            AsnGrnId = asnGrnId,
            IssueSessionId = issueSessionId,
            RequisitionId = requisitionId
        });
        await dbContext.SaveChangesAsync();
    }

    private async Task<(IssueTagScanResponseDto? Scan, string? Error, int StatusCode)> WarningResponseAsync(int sessionId, string tagValue, WarningType warningType, string message, int? rfidTagId = null)
    {
        await dbContext.SaveChangesAsync();
        return (new IssueTagScanResponseDto
        {
            IsValid = false,
            Message = message,
            IssueSessionId = sessionId,
            TagValue = tagValue,
            RfidTagId = rfidTagId,
            WarningType = warningType.ToString()
        }, null, StatusCodes.Status200OK);
    }

    private async Task<IssueSession?> LoadSessionAsync(int id)
    {
        return await dbContext.IssueSessions
            .Include(x => x.PickingList)
            .Include(x => x.StartedByUser)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    private static IssueSessionResponseDto ToSessionResponse(IssueSession session)
    {
        return new IssueSessionResponseDto
        {
            Id = session.Id,
            PickingListId = session.PickingListId,
            PickingListNo = session.PickingList?.PickingListNo ?? string.Empty,
            Status = session.Status.ToString(),
            StartedByUserId = session.StartedByUserId,
            StartedByUsername = session.StartedByUser?.Username ?? string.Empty,
            StartedAt = session.StartedAt,
            LastActivityAt = session.LastActivityAt,
            EndedAt = session.EndedAt
        };
    }
}
