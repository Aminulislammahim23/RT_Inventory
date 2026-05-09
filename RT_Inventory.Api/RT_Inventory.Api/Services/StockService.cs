using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Data;
using RT_Inventory.Api.DTOs.Stocks;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Services;

public class StockService(ApplicationDbContext dbContext) : IStockService
{
    public async Task<IReadOnlyList<StockResponseDto>> GetCurrentAsync(string? status)
    {
        var query = dbContext.Stocks
            .Include(x => x.AsnGrn)
            .Include(x => x.RfidTag)
            .AsQueryable();

        if (Enum.TryParse<StockStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(x => x.Status == parsedStatus);
        }

        return await query.OrderByDescending(x => x.CreatedAt).Select(x => new StockResponseDto
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
        }).ToListAsync();
    }

    public async Task<IReadOnlyList<StockSummaryResponseDto>> GetByItemAsync(string? itemYarnType)
    {
        var query = dbContext.Stocks.AsQueryable();
        if (!string.IsNullOrWhiteSpace(itemYarnType))
        {
            query = query.Where(x => x.ItemYarnType.Contains(itemYarnType));
        }

        return await query
            .GroupBy(x => new { x.ItemYarnType, x.Status })
            .Select(x => new StockSummaryResponseDto
            {
                GroupKey = x.Key.ItemYarnType,
                Status = x.Key.Status.ToString(),
                BagQty = x.Count(),
                TotalWeight = x.Sum(y => y.Weight)
            })
            .OrderBy(x => x.GroupKey)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<StockSummaryResponseDto>> GetByLotAsync(string? lotNo)
    {
        var query = dbContext.Stocks.AsQueryable();
        if (!string.IsNullOrWhiteSpace(lotNo))
        {
            query = query.Where(x => x.LotNo.Contains(lotNo));
        }

        return await query
            .GroupBy(x => new { x.LotNo, x.Status })
            .Select(x => new StockSummaryResponseDto
            {
                GroupKey = x.Key.LotNo,
                Status = x.Key.Status.ToString(),
                BagQty = x.Count(),
                TotalWeight = x.Sum(y => y.Weight)
            })
            .OrderBy(x => x.GroupKey)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<StockTransactionResponseDto>> GetTransactionsAsync(DateTime? fromDate, DateTime? toDate, string? rfidTag)
    {
        var query = dbContext.StockTransactions
            .Include(x => x.RfidTag)
            .Include(x => x.AsnGrn)
            .Include(x => x.CreatedByUser)
            .AsQueryable();

        if (fromDate is not null)
        {
            query = query.Where(x => x.CreatedAt >= fromDate.Value);
        }

        if (toDate is not null)
        {
            query = query.Where(x => x.CreatedAt <= toDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(rfidTag))
        {
            query = query.Where(x => x.RfidTag.TagValue.Contains(rfidTag));
        }

        return await query.OrderByDescending(x => x.CreatedAt).Select(x => new StockTransactionResponseDto
        {
            Id = x.Id,
            RfidTagId = x.RfidTagId,
            TagValue = x.RfidTag.TagValue,
            AsnGrnId = x.AsnGrnId,
            AsnGrnNo = x.AsnGrn == null ? null : x.AsnGrn.AsnGrnNo,
            RequisitionId = x.RequisitionId,
            TransactionType = x.TransactionType.ToString(),
            FromStatus = x.FromStatus.ToString(),
            ToStatus = x.ToStatus.ToString(),
            Weight = x.Weight,
            CreatedByUserId = x.CreatedByUserId,
            CreatedByUsername = x.CreatedByUser.Username,
            CreatedAt = x.CreatedAt
        }).ToListAsync();
    }
}
