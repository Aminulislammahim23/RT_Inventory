using RT_Inventory.Api.DTOs.AsnGrns;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;
using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Services;

public class AsnGrnService(IAsnGrnRepository asnGrnRepository, IUserRepository userRepository) : IAsnGrnService
{
    public async Task<(AsnGrnResponseDto? AsnGrn, string? Error, int StatusCode)> CreateAsync(CreateAsnGrnRequestDto request, int createdByUserId)
    {
        if (await userRepository.GetByIdAsync(createdByUserId) is null)
        {
            return (null, "Authenticated user was not found.", StatusCodes.Status401Unauthorized);
        }

        var asnGrnNo = request.AsnGrnNo.Trim();
        if (await asnGrnRepository.GetByAsnGrnNoAsync(asnGrnNo) is not null)
        {
            return (null, "ASN/GRN number already exists.", StatusCodes.Status409Conflict);
        }

        var asnGrn = new AsnGrn
        {
            AsnGrnNo = asnGrnNo,
            Supplier = request.Supplier.Trim(),
            PoNo = request.PoNo.Trim(),
            ChallanNo = request.ChallanNo.Trim(),
            LotNo = request.LotNo.Trim(),
            ItemYarnType = request.ItemYarnType.Trim(),
            TotalBagQty = request.TotalBagQty,
            WeightPerBag = request.WeightPerBag,
            ChallanCopyPath = NormalizeOptionalPath(request.ChallanCopyPath),
            UsterReportPath = NormalizeOptionalPath(request.UsterReportPath),
            Status = AsnGrnStatus.Parking,
            CreatedByUserId = createdByUserId
        };

        await asnGrnRepository.AddAsync(asnGrn);
        await asnGrnRepository.SaveChangesAsync();

        var created = await asnGrnRepository.GetByIdAsync(asnGrn.Id);
        return (ToResponse(created ?? asnGrn), null, StatusCodes.Status201Created);
    }

    public async Task<IReadOnlyList<AsnGrnResponseDto>> GetAllAsync()
    {
        var asnGrns = await asnGrnRepository.GetAllAsync();
        return asnGrns.Select(ToResponse).ToList();
    }

    public async Task<AsnGrnResponseDto?> GetByIdAsync(int id)
    {
        var asnGrn = await asnGrnRepository.GetByIdAsync(id);
        return asnGrn is null ? null : ToResponse(asnGrn);
    }

    public async Task<(AsnGrnResponseDto? AsnGrn, string? Error, int StatusCode)> UpdateAsync(int id, UpdateAsnGrnRequestDto request)
    {
        var asnGrn = await asnGrnRepository.GetByIdAsync(id);
        if (asnGrn is null)
        {
            return (null, "ASN/GRN was not found.", StatusCodes.Status404NotFound);
        }

        if (asnGrn.Status != AsnGrnStatus.Parking)
        {
            return (null, "Only ASN/GRN records in Parking status can be updated.", StatusCodes.Status400BadRequest);
        }

        asnGrn.Supplier = request.Supplier.Trim();
        asnGrn.PoNo = request.PoNo.Trim();
        asnGrn.ChallanNo = request.ChallanNo.Trim();
        asnGrn.LotNo = request.LotNo.Trim();
        asnGrn.ItemYarnType = request.ItemYarnType.Trim();
        asnGrn.TotalBagQty = request.TotalBagQty;
        asnGrn.WeightPerBag = request.WeightPerBag;
        asnGrn.ChallanCopyPath = NormalizeOptionalPath(request.ChallanCopyPath);
        asnGrn.UsterReportPath = NormalizeOptionalPath(request.UsterReportPath);
        asnGrn.UpdatedAt = DateTime.UtcNow;

        await asnGrnRepository.SaveChangesAsync();
        return (ToResponse(asnGrn), null, StatusCodes.Status200OK);
    }

    private static string? NormalizeOptionalPath(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static AsnGrnResponseDto ToResponse(AsnGrn asnGrn)
    {
        return new AsnGrnResponseDto
        {
            Id = asnGrn.Id,
            AsnGrnNo = asnGrn.AsnGrnNo,
            Supplier = asnGrn.Supplier,
            PoNo = asnGrn.PoNo,
            ChallanNo = asnGrn.ChallanNo,
            LotNo = asnGrn.LotNo,
            ItemYarnType = asnGrn.ItemYarnType,
            TotalBagQty = asnGrn.TotalBagQty,
            WeightPerBag = asnGrn.WeightPerBag,
            TotalWeight = asnGrn.TotalBagQty * asnGrn.WeightPerBag,
            ChallanCopyPath = asnGrn.ChallanCopyPath,
            UsterReportPath = asnGrn.UsterReportPath,
            Status = asnGrn.Status.ToString(),
            CreatedByUserId = asnGrn.CreatedByUserId,
            CreatedByUsername = asnGrn.CreatedByUser?.Username ?? string.Empty,
            CreatedAt = asnGrn.CreatedAt,
            UpdatedAt = asnGrn.UpdatedAt
        };
    }
}
