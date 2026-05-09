using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Data;
using RT_Inventory.Api.DTOs.Returns;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;
using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Services;

public class ReturnService(ApplicationDbContext dbContext) : IReturnService
{
    public async Task<(ReturnRequestResponseDto? ReturnRequest, string? Error, int StatusCode)> CreateAsync(CreateReturnRequestDto request, int createdByUserId)
    {
        if (request.SourceRequisitionId is not null && !await dbContext.Requisitions.AnyAsync(x => x.Id == request.SourceRequisitionId))
        {
            return (null, "Source requisition was not found.", StatusCodes.Status404NotFound);
        }

        var returnRequest = new ReturnRequest
        {
            ReturnNo = await GenerateReturnNoAsync(),
            SourceRequisitionId = request.SourceRequisitionId,
            Status = ReturnRequestStatus.Pending,
            CreatedByUserId = createdByUserId
        };

        dbContext.ReturnRequests.Add(returnRequest);
        await dbContext.SaveChangesAsync();
        var loaded = await LoadAsync(returnRequest.Id);
        return (ToResponse(loaded ?? returnRequest), null, StatusCodes.Status201Created);
    }

    public async Task<IReadOnlyList<ReturnRequestResponseDto>> GetAllAsync()
    {
        var returns = await BaseReturnQuery()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return returns.Select(ToResponse).ToList();
    }

    public async Task<(ReturnRequestResponseDto? ReturnRequest, string? Error, int StatusCode)> CreateReturnAsnGrnAsync(int id, CreateReturnAsnGrnRequestDto request, int createdByUserId)
    {
        var returnRequest = await LoadAsync(id);
        if (returnRequest is null)
        {
            return (null, "Return request was not found.", StatusCodes.Status404NotFound);
        }

        if (returnRequest.ReturnAsnGrnId is not null)
        {
            return (null, "Return ASN/GRN is already created.", StatusCodes.Status409Conflict);
        }

        var asn = new AsnGrn
        {
            AsnGrnNo = $"RTN-GRN-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Supplier = "Return",
            PoNo = returnRequest.SourceRequisitionId?.ToString() ?? "RETURN",
            ChallanNo = returnRequest.ReturnNo,
            LotNo = request.LotNo.Trim(),
            ItemYarnType = request.ItemYarnType.Trim(),
            TotalBagQty = request.TotalBagQty,
            WeightPerBag = request.WeightPerBag,
            Status = AsnGrnStatus.Parking,
            CreatedByUserId = createdByUserId
        };

        dbContext.AsnGrns.Add(asn);
        await dbContext.SaveChangesAsync();

        returnRequest.ReturnAsnGrnId = asn.Id;
        returnRequest.Status = ReturnRequestStatus.AsnGrnCreated;
        await dbContext.SaveChangesAsync();

        var loaded = await LoadAsync(id);
        return (ToResponse(loaded ?? returnRequest), null, StatusCodes.Status201Created);
    }

    private async Task<ReturnRequest?> LoadAsync(int id)
    {
        return await BaseReturnQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    private IQueryable<ReturnRequest> BaseReturnQuery()
    {
        return dbContext.ReturnRequests
            .Include(x => x.CreatedByUser)
            .Include(x => x.ReturnAsnGrn);
    }

    private async Task<string> GenerateReturnNoAsync()
    {
        var prefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await dbContext.ReturnRequests.CountAsync(x => x.ReturnNo.StartsWith($"RET-{prefix}"));
        return $"RET-{prefix}-{count + 1:0000}";
    }

    private static ReturnRequestResponseDto ToResponse(ReturnRequest returnRequest)
    {
        return new ReturnRequestResponseDto
        {
            Id = returnRequest.Id,
            ReturnNo = returnRequest.ReturnNo,
            SourceRequisitionId = returnRequest.SourceRequisitionId,
            ReturnAsnGrnId = returnRequest.ReturnAsnGrnId,
            ReturnAsnGrnNo = returnRequest.ReturnAsnGrn?.AsnGrnNo,
            Status = returnRequest.Status.ToString(),
            CreatedByUserId = returnRequest.CreatedByUserId,
            CreatedByUsername = returnRequest.CreatedByUser?.Username ?? string.Empty,
            CreatedAt = returnRequest.CreatedAt
        };
    }
}
