using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Data;
using RT_Inventory.Api.DTOs.MrrApprovals;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;
using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Services;

public class MrrApprovalService(ApplicationDbContext dbContext) : IMrrApprovalService
{
    public async Task<(MrrApprovalResponseDto? Approval, string? Error, int StatusCode)> ApproveAsync(int asnGrnId, int approvedByUserId)
    {
        var asnGrn = await dbContext.AsnGrns.FirstOrDefaultAsync(x => x.Id == asnGrnId);
        if (asnGrn is null)
        {
            return (null, "ASN/GRN was not found.", StatusCodes.Status404NotFound);
        }

        var duplicate = await dbContext.MrrApprovals.AnyAsync(x => x.AsnGrnId == asnGrnId);
        if (duplicate || asnGrn.Status == AsnGrnStatus.Confirmed)
        {
            return (null, "ASN/GRN is already approved.", StatusCodes.Status409Conflict);
        }

        var stocks = await dbContext.Stocks.Where(x => x.AsnGrnId == asnGrnId && x.Status == StockStatus.Parking).ToListAsync();
        if (stocks.Count == 0)
        {
            return (null, "No Parking Stock was found for this ASN/GRN.", StatusCodes.Status400BadRequest);
        }

        var approval = new MrrApproval
        {
            AsnGrnId = asnGrnId,
            MrrNo = await GenerateMrrNoAsync(),
            ApprovedByUserId = approvedByUserId,
            ApprovedAt = DateTime.UtcNow
        };

        dbContext.MrrApprovals.Add(approval);
        asnGrn.Status = AsnGrnStatus.Confirmed;
        asnGrn.UpdatedAt = DateTime.UtcNow;

        foreach (var stock in stocks)
        {
            stock.Status = StockStatus.Confirmed;
            stock.UpdatedAt = DateTime.UtcNow;
            dbContext.StockTransactions.Add(new StockTransaction
            {
                RfidTagId = stock.RfidTagId,
                AsnGrnId = asnGrn.Id,
                TransactionType = StockTransactionType.Approval,
                FromStatus = StockStatus.Parking,
                ToStatus = StockStatus.Confirmed,
                Weight = stock.Weight,
                CreatedByUserId = approvedByUserId
            });
        }

        await dbContext.SaveChangesAsync();

        var loaded = await dbContext.MrrApprovals
            .Include(x => x.AsnGrn)
            .Include(x => x.ApprovedByUser)
            .FirstAsync(x => x.Id == approval.Id);

        return (new MrrApprovalResponseDto
        {
            Id = loaded.Id,
            AsnGrnId = loaded.AsnGrnId,
            AsnGrnNo = loaded.AsnGrn.AsnGrnNo,
            MrrNo = loaded.MrrNo,
            ApprovedByUserId = loaded.ApprovedByUserId,
            ApprovedByUsername = loaded.ApprovedByUser.Username,
            ApprovedAt = loaded.ApprovedAt,
            ConfirmedBagQty = stocks.Count,
            ConfirmedWeight = stocks.Sum(x => x.Weight)
        }, null, StatusCodes.Status200OK);
    }

    private async Task<string> GenerateMrrNoAsync()
    {
        var datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await dbContext.MrrApprovals.CountAsync(x => x.MrrNo.StartsWith($"MRR-{datePrefix}"));
        return $"MRR-{datePrefix}-{count + 1:0000}";
    }
}
