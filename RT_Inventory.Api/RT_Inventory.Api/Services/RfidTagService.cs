using RT_Inventory.Api.DTOs.RfidTags;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;
using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Services;

public class RfidTagService(IRfidTagRepository rfidTagRepository, IAsnGrnRepository asnGrnRepository) : IRfidTagService
{
    public async Task<IReadOnlyList<RfidTagResponseDto>> GetAllAsync()
    {
        var tags = await rfidTagRepository.GetAllAsync();
        return tags.Select(ToResponse).ToList();
    }

    public async Task<(RfidTagResponseDto? Tag, string? Error, int StatusCode)> ScanAsync(RfidTagScanRequestDto request, int performedByUserId)
    {
        var tagValue = request.TagValue.Trim();
        var activeTag = await rfidTagRepository.GetActiveByTagValueAsync(tagValue);
        if (activeTag is not null)
        {
            return (null, "Active RFID tag already exists.", StatusCodes.Status409Conflict);
        }

        var rfidTag = new RfidTag
        {
            TagValue = tagValue,
            AssignmentType = RfidTagAssignmentType.LooseBag,
            IsActive = true
        };

        await rfidTagRepository.AddAsync(rfidTag);
        await rfidTagRepository.SaveChangesAsync();

        await AddHistoryAsync(rfidTag.Id, "Scanned", Normalize(request.Remarks), performedByUserId);
        return (ToResponse(rfidTag), null, StatusCodes.Status201Created);
    }

    public async Task<(RfidTagResponseDto? Tag, string? Error, int StatusCode)> AssignAsync(AssignRfidTagRequestDto request, int performedByUserId)
    {
        var tagValue = request.TagValue.Trim();
        var rfidTag = await rfidTagRepository.GetActiveByTagValueAsync(tagValue);
        if (rfidTag is null)
        {
            rfidTag = new RfidTag
            {
                TagValue = tagValue,
                IsActive = true
            };
            await rfidTagRepository.AddAsync(rfidTag);
            await rfidTagRepository.SaveChangesAsync();
            await AddHistoryAsync(rfidTag.Id, "Scanned", "Created during assignment.", performedByUserId);
        }

        if (!request.IsLooseBag)
        {
            if (request.AsnGrnId is null || request.BagNumber is null)
            {
                return (null, "ASN/GRN id and bag number are required for ASN/GRN bag assignment.", StatusCodes.Status400BadRequest);
            }

            var asnGrn = await asnGrnRepository.GetByIdAsync(request.AsnGrnId.Value);
            if (asnGrn is null)
            {
                return (null, "ASN/GRN was not found.", StatusCodes.Status404NotFound);
            }

            if (request.BagNumber < 1 || request.BagNumber > asnGrn.TotalBagQty)
            {
                return (null, "Bag number must be within ASN/GRN total bag quantity.", StatusCodes.Status400BadRequest);
            }

            rfidTag.AsnGrnId = asnGrn.Id;
            rfidTag.AsnGrn = asnGrn;
            rfidTag.BagNumber = request.BagNumber;
            rfidTag.AssignmentType = RfidTagAssignmentType.AsnGrnBag;
        }
        else
        {
            rfidTag.AsnGrnId = null;
            rfidTag.AsnGrn = null;
            rfidTag.BagNumber = null;
            rfidTag.AssignmentType = RfidTagAssignmentType.LooseBag;
        }

        rfidTag.UpdatedAt = DateTime.UtcNow;
        await rfidTagRepository.SaveChangesAsync();
        await AddHistoryAsync(rfidTag.Id, "Assigned", Normalize(request.Remarks), performedByUserId);

        var updated = await rfidTagRepository.GetByIdAsync(rfidTag.Id);
        return (ToResponse(updated ?? rfidTag), null, StatusCodes.Status200OK);
    }

    public async Task<(RfidTagResponseDto? Tag, string? Error, int StatusCode)> SetStatusAsync(int id, bool isActive, int performedByUserId)
    {
        var rfidTag = await rfidTagRepository.GetByIdAsync(id);
        if (rfidTag is null)
        {
            return (null, "RFID tag was not found.", StatusCodes.Status404NotFound);
        }

        if (isActive)
        {
            var activeTag = await rfidTagRepository.GetActiveByTagValueAsync(rfidTag.TagValue);
            if (activeTag is not null && activeTag.Id != id)
            {
                return (null, "Another active RFID tag with the same value already exists.", StatusCodes.Status409Conflict);
            }
        }

        rfidTag.IsActive = isActive;
        rfidTag.UpdatedAt = DateTime.UtcNow;
        await rfidTagRepository.SaveChangesAsync();
        await AddHistoryAsync(rfidTag.Id, isActive ? "Activated" : "Deactivated", null, performedByUserId);

        return (ToResponse(rfidTag), null, StatusCodes.Status200OK);
    }

    public async Task<(IReadOnlyList<RfidTagHistoryResponseDto>? History, string? Error, int StatusCode)> GetHistoryAsync(int id)
    {
        var rfidTag = await rfidTagRepository.GetByIdAsync(id);
        if (rfidTag is null)
        {
            return (null, "RFID tag was not found.", StatusCodes.Status404NotFound);
        }

        var history = await rfidTagRepository.GetHistoryAsync(id);
        return (history.Select(ToHistoryResponse).ToList(), null, StatusCodes.Status200OK);
    }

    private async Task AddHistoryAsync(int rfidTagId, string action, string? remarks, int performedByUserId)
    {
        await rfidTagRepository.AddHistoryAsync(new RfidTagHistory
        {
            RfidTagId = rfidTagId,
            Action = action,
            Remarks = remarks,
            PerformedByUserId = performedByUserId
        });
        await rfidTagRepository.SaveChangesAsync();
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static RfidTagResponseDto ToResponse(RfidTag rfidTag)
    {
        return new RfidTagResponseDto
        {
            Id = rfidTag.Id,
            TagValue = rfidTag.TagValue,
            AsnGrnId = rfidTag.AsnGrnId,
            AsnGrnNo = rfidTag.AsnGrn?.AsnGrnNo,
            BagNumber = rfidTag.BagNumber,
            AssignmentType = rfidTag.AssignmentType.ToString(),
            IsActive = rfidTag.IsActive,
            CreatedAt = rfidTag.CreatedAt,
            UpdatedAt = rfidTag.UpdatedAt
        };
    }

    private static RfidTagHistoryResponseDto ToHistoryResponse(RfidTagHistory history)
    {
        return new RfidTagHistoryResponseDto
        {
            Id = history.Id,
            RfidTagId = history.RfidTagId,
            TagValue = history.RfidTag.TagValue,
            Action = history.Action,
            Remarks = history.Remarks,
            PerformedByUserId = history.PerformedByUserId,
            PerformedByUsername = history.PerformedByUser?.Username,
            CreatedAt = history.CreatedAt
        };
    }
}
