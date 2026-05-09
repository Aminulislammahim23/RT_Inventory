using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Data;
using RT_Inventory.Api.DTOs.Reports;
using RT_Inventory.Api.DTOs.RfidTags;
using RT_Inventory.Api.DTOs.Stocks;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Services;

public class ReportService(ApplicationDbContext dbContext) : IReportService
{
    public async Task<IReadOnlyList<StockResponseDto>> CurrentStockAsync(string? itemType, string? lot, string? asn)
    {
        var query = BaseStockQuery();
        if (!string.IsNullOrWhiteSpace(itemType)) query = query.Where(x => x.ItemYarnType.Contains(itemType));
        if (!string.IsNullOrWhiteSpace(lot)) query = query.Where(x => x.LotNo.Contains(lot));
        if (!string.IsNullOrWhiteSpace(asn)) query = query.Where(x => x.AsnGrn.AsnGrnNo.Contains(asn));
        return await query.Select(x => ToStockResponse(x)).ToListAsync();
    }

    public async Task<IReadOnlyList<StockResponseDto>> StockByStatusAsync(string status)
    {
        if (!Enum.TryParse<StockStatus>(status, true, out var parsed)) return [];
        return await BaseStockQuery().Where(x => x.Status == parsed).Select(x => ToStockResponse(x)).ToListAsync();
    }

    public async Task<IReadOnlyList<AsnReceiveReportDto>> AsnWiseReceiveAsync(DateTime? fromDate, DateTime? toDate, string? supplier, string? lot, string? itemType, string? asn)
    {
        var query = dbContext.Stocks.Include(x => x.AsnGrn).AsQueryable();
        if (fromDate is not null) query = query.Where(x => x.CreatedAt >= fromDate.Value);
        if (toDate is not null) query = query.Where(x => x.CreatedAt <= toDate.Value);
        if (!string.IsNullOrWhiteSpace(supplier)) query = query.Where(x => x.AsnGrn.Supplier.Contains(supplier));
        if (!string.IsNullOrWhiteSpace(lot)) query = query.Where(x => x.LotNo.Contains(lot));
        if (!string.IsNullOrWhiteSpace(itemType)) query = query.Where(x => x.ItemYarnType.Contains(itemType));
        if (!string.IsNullOrWhiteSpace(asn)) query = query.Where(x => x.AsnGrn.AsnGrnNo.Contains(asn));

        return await query.GroupBy(x => new { x.AsnGrnId, x.AsnGrn.AsnGrnNo, x.AsnGrn.Supplier, x.LotNo, x.ItemYarnType })
            .Select(x => new AsnReceiveReportDto
            {
                AsnGrnId = x.Key.AsnGrnId,
                AsnGrnNo = x.Key.AsnGrnNo,
                Supplier = x.Key.Supplier,
                LotNo = x.Key.LotNo,
                ItemYarnType = x.Key.ItemYarnType,
                ReceivedBagQty = x.Count(),
                ReceivedWeight = x.Sum(y => y.Weight)
            }).ToListAsync();
    }

    public async Task<IReadOnlyList<RequisitionIssueReportDto>> RequisitionWiseIssueAsync(DateTime? fromDate, DateTime? toDate, string? requisition, string? rfidTag)
    {
        var query = dbContext.StockTransactions
            .Include(x => x.Requisition)
            .Include(x => x.RfidTag)
            .Where(x => x.TransactionType == StockTransactionType.Issue && x.RequisitionId != null)
            .AsQueryable();

        if (fromDate is not null) query = query.Where(x => x.CreatedAt >= fromDate.Value);
        if (toDate is not null) query = query.Where(x => x.CreatedAt <= toDate.Value);
        if (!string.IsNullOrWhiteSpace(requisition)) query = query.Where(x => x.Requisition!.RequisitionNo.Contains(requisition));
        if (!string.IsNullOrWhiteSpace(rfidTag)) query = query.Where(x => x.RfidTag.TagValue.Contains(rfidTag));

        return await query.GroupBy(x => new { x.RequisitionId, x.Requisition!.RequisitionNo })
            .Select(x => new RequisitionIssueReportDto
            {
                RequisitionId = x.Key.RequisitionId!.Value,
                RequisitionNo = x.Key.RequisitionNo,
                IssuedBagQty = x.Count(),
                IssuedWeight = x.Sum(y => y.Weight)
            }).ToListAsync();
    }

    public async Task<IReadOnlyList<RfidTagHistoryResponseDto>> RfidTagHistoryAsync(string? rfidTag)
    {
        var query = dbContext.RfidTagHistories.Include(x => x.RfidTag).Include(x => x.PerformedByUser).AsQueryable();
        if (!string.IsNullOrWhiteSpace(rfidTag)) query = query.Where(x => x.RfidTag.TagValue.Contains(rfidTag));
        return await query.OrderByDescending(x => x.CreatedAt).Select(x => new RfidTagHistoryResponseDto
        {
            Id = x.Id,
            RfidTagId = x.RfidTagId,
            TagValue = x.RfidTag.TagValue,
            Action = x.Action,
            Remarks = x.Remarks,
            PerformedByUserId = x.PerformedByUserId,
            PerformedByUsername = x.PerformedByUser == null ? null : x.PerformedByUser.Username,
            CreatedAt = x.CreatedAt
        }).ToListAsync();
    }

    public async Task<IReadOnlyList<WarningReportDto>> WarningsAsync(DateTime? fromDate, DateTime? toDate, string? asn, string? requisition, string? rfidTag)
    {
        var query = dbContext.WarningLogs.Include(x => x.AsnGrn).Include(x => x.Requisition).AsQueryable();
        if (fromDate is not null) query = query.Where(x => x.CreatedAt >= fromDate.Value);
        if (toDate is not null) query = query.Where(x => x.CreatedAt <= toDate.Value);
        if (!string.IsNullOrWhiteSpace(asn)) query = query.Where(x => x.AsnGrn != null && x.AsnGrn.AsnGrnNo.Contains(asn));
        if (!string.IsNullOrWhiteSpace(requisition)) query = query.Where(x => x.Requisition != null && x.Requisition.RequisitionNo.Contains(requisition));
        if (!string.IsNullOrWhiteSpace(rfidTag)) query = query.Where(x => x.RfidTagValue != null && x.RfidTagValue.Contains(rfidTag));

        return await query.OrderByDescending(x => x.CreatedAt).Select(x => new WarningReportDto
        {
            Id = x.Id,
            WarningType = x.WarningType.ToString(),
            Message = x.Message,
            RfidTagValue = x.RfidTagValue,
            AsnGrnId = x.AsnGrnId,
            RequisitionId = x.RequisitionId,
            ReceiveSessionId = x.ReceiveSessionId,
            IssueSessionId = x.IssueSessionId,
            CreatedAt = x.CreatedAt
        }).ToListAsync();
    }

    private IQueryable<Models.Entities.Stock> BaseStockQuery()
    {
        return dbContext.Stocks.Include(x => x.AsnGrn).Include(x => x.RfidTag).OrderByDescending(x => x.CreatedAt);
    }

    private static StockResponseDto ToStockResponse(Models.Entities.Stock x)
    {
        return new StockResponseDto
        {
            Id = x.Id,
            AsnGrnId = x.AsnGrnId,
            AsnGrnNo = x.AsnGrn.AsnGrnNo,
            RfidTagId = x.RfidTagId,
            TagValue = x.RfidTag.TagValue,
            LotNo = x.LotNo,
            ItemYarnType = x.ItemYarnType,
            Weight = x.Weight,
            Status = x.Status.ToString(),
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        };
    }
}
