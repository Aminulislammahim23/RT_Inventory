using System.ComponentModel.DataAnnotations;

namespace RT_Inventory.Api.DTOs.RfidTags;

public class AssignRfidTagRequestDto
{
    [Required]
    [StringLength(120)]
    public string TagValue { get; set; } = string.Empty;

    public int? AsnGrnId { get; set; }

    public int? BagNumber { get; set; }

    public bool IsLooseBag { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }
}
