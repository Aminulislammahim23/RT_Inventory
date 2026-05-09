using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Data;
using RT_Inventory.Api.DTOs.ReceiveSessions;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;
using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Services;

public class ReceiveSessionService(ApplicationDbContext dbContext) : IReceiveSessionService
{
    private static readonly TimeSpan InactivityTimeout = TimeSpan.FromMinutes(15);

    public async Task<(ReceiveSessionResponseDto? Session, string? Error, int StatusCode)> StartAsync(StartReceiveSessionRequestDto request, int startedByUserId)
    {
        var asnGrnNo = request.AsnGrnNo?.Trim();
        var asnGrn = request.AsnGrnId is > 0
            ? await dbContext.AsnGrns.FirstOrDefaultAsync(x => x.Id == request.AsnGrnId.Value)
            : await dbContext.AsnGrns.FirstOrDefaultAsync(x => x.AsnGrnNo == asnGrnNo);

        if (asnGrn is null)
        {
            await AddWarningAsync(WarningType.AsnGrnNotScanned, "ASN/GRN was not found during receive session start.", asnGrnNo, request.AsnGrnId, null, null);
            return (null, "ASN/GRN was not found.", StatusCodes.Status404NotFound);
        }

        var session = new ReceiveSession
        {
            AsnGrnId = asnGrn.Id,
            StartedByUserId = startedByUserId,
            Status = GateSessionStatus.Active,
            StartedAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow
        };

        dbContext.ReceiveSessions.Add(session);
        await dbContext.SaveChangesAsync();

        var loaded = await LoadSessionAsync(session.Id);
        return (ToSessionResponse(loaded ?? session), null, StatusCodes.Status201Created);
    }

    public async Task<(ReceiveTagScanResponseDto? Scan, string? Error, int StatusCode)> ScanAsync(int sessionId, ReceiveTagScanRequestDto request, int performedByUserId)
    {
        var session = await dbContext.ReceiveSessions
            .Include(x => x.AsnGrn)
            .FirstOrDefaultAsync(x => x.Id == sessionId);

        if (session is null)
        {
            return (null, "Receive session was not found.", StatusCodes.Status404NotFound);
        }

        if (session.Status != GateSessionStatus.Active || DateTime.UtcNow - session.LastActivityAt > InactivityTimeout)
        {
            session.Status = GateSessionStatus.Expired;
            session.EndedAt ??= DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return (null, "Receive session is not active or has expired.", StatusCodes.Status400BadRequest);
        }

        session.LastActivityAt = DateTime.UtcNow;
        var tagValue = request.TagValue.Trim();
        var tag = await dbContext.RfidTags.Include(x => x.AsnGrn).FirstOrDefaultAsync(x => x.TagValue == tagValue && x.IsActive);

        if (tag is null)
        {
            await AddWarningAsync(WarningType.MismatchedRfidTag, "RFID tag is not active or was not found.", tagValue, session.AsnGrnId, session.Id, null);
            return await SaveWarningResponseAsync(session.Id, tagValue, WarningType.MismatchedRfidTag, "RFID tag is not active or was not found.");
        }

        if (tag.AsnGrnId is null)
        {
            await AddWarningAsync(WarningType.ExtraBagDetected, "Loose bag tag detected during ASN/GRN receive.", tagValue, session.AsnGrnId, session.Id, null);
            return await SaveWarningResponseAsync(session.Id, tagValue, WarningType.ExtraBagDetected, "Loose bag tag detected during ASN/GRN receive.", tag.Id);
        }

        if (tag.AsnGrnId != session.AsnGrnId)
        {
            await AddWarningAsync(WarningType.MismatchedRfidTag, "RFID tag belongs to another ASN/GRN.", tagValue, session.AsnGrnId, session.Id, null);
            return await SaveWarningResponseAsync(session.Id, tagValue, WarningType.MismatchedRfidTag, "RFID tag belongs to another ASN/GRN.", tag.Id);
        }

        var stock = await dbContext.Stocks.FirstOrDefaultAsync(x => x.RfidTagId == tag.Id);
        if (stock is null)
        {
            stock = new Stock
            {
                AsnGrnId = session.AsnGrnId,
                RfidTagId = tag.Id,
                LotNo = session.AsnGrn.LotNo,
                ItemYarnType = session.AsnGrn.ItemYarnType,
                Weight = session.AsnGrn.WeightPerBag,
                Status = StockStatus.Parking
            };
            dbContext.Stocks.Add(stock);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            stock.Status = StockStatus.Parking;
            stock.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }

        dbContext.StockTransactions.Add(new StockTransaction
        {
            RfidTagId = tag.Id,
            AsnGrnId = session.AsnGrnId,
            TransactionType = StockTransactionType.Receive,
            FromStatus = StockStatus.Parking,
            ToStatus = StockStatus.Parking,
            Weight = stock.Weight,
            CreatedByUserId = performedByUserId
        });
        await dbContext.SaveChangesAsync();

        return (new ReceiveTagScanResponseDto
        {
            IsValid = true,
            Message = "RFID tag received and Parking Stock updated.",
            ReceiveSessionId = session.Id,
            TagValue = tagValue,
            RfidTagId = tag.Id,
            StockId = stock.Id
        }, null, StatusCodes.Status200OK);
    }

    public async Task<(ReceiveSessionResponseDto? Session, string? Error, int StatusCode)> DeactivateAsync(int sessionId)
    {
        var session = await LoadSessionAsync(sessionId);
        if (session is null)
        {
            return (null, "Receive session was not found.", StatusCodes.Status404NotFound);
        }

        session.Status = GateSessionStatus.Deactivated;
        session.EndedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return (ToSessionResponse(session), null, StatusCodes.Status200OK);
    }

    private async Task<ReceiveSession?> LoadSessionAsync(int id)
    {
        return await dbContext.ReceiveSessions
            .Include(x => x.AsnGrn)
            .Include(x => x.StartedByUser)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    private async Task AddWarningAsync(WarningType warningType, string message, string? tagValue, int? asnGrnId, int? receiveSessionId, int? issueSessionId)
    {
        dbContext.WarningLogs.Add(new WarningLog
        {
            WarningType = warningType,
            Message = message,
            RfidTagValue = tagValue,
            AsnGrnId = asnGrnId,
            ReceiveSessionId = receiveSessionId,
            IssueSessionId = issueSessionId
        });
        await dbContext.SaveChangesAsync();
    }

    private async Task<(ReceiveTagScanResponseDto? Scan, string? Error, int StatusCode)> SaveWarningResponseAsync(int sessionId, string tagValue, WarningType warningType, string message, int? rfidTagId = null)
    {
        await dbContext.SaveChangesAsync();
        return (new ReceiveTagScanResponseDto
        {
            IsValid = false,
            Message = message,
            ReceiveSessionId = sessionId,
            TagValue = tagValue,
            RfidTagId = rfidTagId,
            WarningType = warningType.ToString()
        }, null, StatusCodes.Status200OK);
    }

    private static ReceiveSessionResponseDto ToSessionResponse(ReceiveSession session)
    {
        return new ReceiveSessionResponseDto
        {
            Id = session.Id,
            AsnGrnId = session.AsnGrnId,
            AsnGrnNo = session.AsnGrn?.AsnGrnNo ?? string.Empty,
            Status = session.Status.ToString(),
            StartedByUserId = session.StartedByUserId,
            StartedByUsername = session.StartedByUser?.Username ?? string.Empty,
            StartedAt = session.StartedAt,
            LastActivityAt = session.LastActivityAt,
            EndedAt = session.EndedAt
        };
    }
}
