using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Data;
using RT_Inventory.Api.DTOs.Requisitions;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;
using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Services;

public class RequisitionService(ApplicationDbContext dbContext) : IRequisitionService
{
    public async Task<(RequisitionResponseDto? Requisition, string? Error, int StatusCode)> CreateAsync(CreateRequisitionRequestDto request, int createdByUserId)
    {
        if (!Enum.TryParse<RequisitionPurpose>(request.Purpose, true, out var purpose))
        {
            return (null, "Requisition purpose must be Knitting, YD, or Testing.", StatusCodes.Status400BadRequest);
        }

        var requisition = new Requisition
        {
            RequisitionNo = await GenerateRequisitionNoAsync(),
            Purpose = purpose,
            Status = RequisitionStatus.Pending,
            CreatedByUserId = createdByUserId,
            Items = request.Items.Select(x => new RequisitionItem
            {
                ItemYarnType = x.ItemYarnType.Trim(),
                LotNo = string.IsNullOrWhiteSpace(x.LotNo) ? null : x.LotNo.Trim(),
                RequiredBagQty = x.RequiredBagQty,
                RequiredWeight = x.RequiredWeight
            }).ToList()
        };

        dbContext.Requisitions.Add(requisition);
        await dbContext.SaveChangesAsync();

        var loaded = await LoadRequisitionAsync(requisition.Id);
        return (ToResponse(loaded ?? requisition), null, StatusCodes.Status201Created);
    }

    public async Task<RequisitionResponseDto?> GetByIdAsync(int id)
    {
        var requisition = await LoadRequisitionAsync(id);
        return requisition is null ? null : ToResponse(requisition);
    }

    public async Task<IReadOnlyList<RequisitionResponseDto>> GetAllAsync()
    {
        var requisitions = await BaseRequisitionQuery()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return requisitions.Select(ToResponse).ToList();
    }

    public async Task<(PickingListResponseDto? PickingList, string? Error, int StatusCode)> GeneratePickingListAsync(int requisitionId, int createdByUserId)
    {
        var requisition = await dbContext.Requisitions.Include(x => x.PickingLists).FirstOrDefaultAsync(x => x.Id == requisitionId);
        if (requisition is null)
        {
            return (null, "Requisition was not found.", StatusCodes.Status404NotFound);
        }

        if (requisition.Status == RequisitionStatus.Cancelled || requisition.Status == RequisitionStatus.Issued)
        {
            return (null, "Picking list cannot be generated for cancelled or issued requisition.", StatusCodes.Status400BadRequest);
        }

        var pickingList = new PickingList
        {
            RequisitionId = requisitionId,
            PickingListNo = await GeneratePickingListNoAsync(),
            BarcodeValue = $"PL|{requisition.RequisitionNo}|{Guid.NewGuid():N}",
            CreatedByUserId = createdByUserId
        };

        requisition.Status = RequisitionStatus.Picked;
        requisition.UpdatedAt = DateTime.UtcNow;
        dbContext.PickingLists.Add(pickingList);
        await dbContext.SaveChangesAsync();

        var loaded = await dbContext.PickingLists.Include(x => x.CreatedByUser).FirstAsync(x => x.Id == pickingList.Id);
        return (ToPickingListResponse(loaded), null, StatusCodes.Status201Created);
    }

    private async Task<Requisition?> LoadRequisitionAsync(int id)
    {
        return await BaseRequisitionQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    private IQueryable<Requisition> BaseRequisitionQuery()
    {
        return dbContext.Requisitions
            .Include(x => x.CreatedByUser)
            .Include(x => x.Items)
            .Include(x => x.PickingLists).ThenInclude(x => x.CreatedByUser);
    }

    private async Task<string> GenerateRequisitionNoAsync()
    {
        var prefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await dbContext.Requisitions.CountAsync(x => x.RequisitionNo.StartsWith($"REQ-{prefix}"));
        return $"REQ-{prefix}-{count + 1:0000}";
    }

    private async Task<string> GeneratePickingListNoAsync()
    {
        var prefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await dbContext.PickingLists.CountAsync(x => x.PickingListNo.StartsWith($"PL-{prefix}"));
        return $"PL-{prefix}-{count + 1:0000}";
    }

    private static RequisitionResponseDto ToResponse(Requisition requisition)
    {
        return new RequisitionResponseDto
        {
            Id = requisition.Id,
            RequisitionNo = requisition.RequisitionNo,
            Purpose = requisition.Purpose.ToString(),
            Status = requisition.Status.ToString(),
            CreatedByUserId = requisition.CreatedByUserId,
            CreatedByUsername = requisition.CreatedByUser?.Username ?? string.Empty,
            CreatedAt = requisition.CreatedAt,
            UpdatedAt = requisition.UpdatedAt,
            Items = requisition.Items.Select(x => new RequisitionItemResponseDto
            {
                Id = x.Id,
                ItemYarnType = x.ItemYarnType,
                LotNo = x.LotNo,
                RequiredBagQty = x.RequiredBagQty,
                RequiredWeight = x.RequiredWeight
            }).ToList(),
            PickingLists = requisition.PickingLists.Select(ToPickingListResponse).ToList()
        };
    }

    private static PickingListResponseDto ToPickingListResponse(PickingList pickingList)
    {
        return new PickingListResponseDto
        {
            Id = pickingList.Id,
            RequisitionId = pickingList.RequisitionId,
            PickingListNo = pickingList.PickingListNo,
            BarcodeValue = pickingList.BarcodeValue,
            CreatedByUserId = pickingList.CreatedByUserId,
            CreatedByUsername = pickingList.CreatedByUser?.Username ?? string.Empty,
            CreatedAt = pickingList.CreatedAt
        };
    }
}
