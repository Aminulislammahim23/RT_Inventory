using System.ComponentModel.DataAnnotations;

namespace RT_Inventory.Api.DTOs.RfidTags;

public class RfidTagScanRequestDto
{
    [Required]
    [StringLength(120)]
    public string TagValue { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Remarks { get; set; }
}
