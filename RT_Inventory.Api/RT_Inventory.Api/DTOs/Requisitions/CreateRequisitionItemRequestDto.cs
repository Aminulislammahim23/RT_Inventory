using System.ComponentModel.DataAnnotations;

namespace RT_Inventory.Api.DTOs.Requisitions;

public class CreateRequisitionItemRequestDto
{
    [Required]
    [StringLength(120)]
    public string ItemYarnType { get; set; } = string.Empty;

    [StringLength(80)]
    public string? LotNo { get; set; }

    [Range(1, int.MaxValue)]
    public int RequiredBagQty { get; set; }

    [Range(0.001, 999999999)]
    public decimal RequiredWeight { get; set; }
}
