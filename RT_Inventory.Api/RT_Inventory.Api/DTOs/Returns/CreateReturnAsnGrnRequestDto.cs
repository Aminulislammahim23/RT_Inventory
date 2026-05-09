using System.ComponentModel.DataAnnotations;

namespace RT_Inventory.Api.DTOs.Returns;

public class CreateReturnAsnGrnRequestDto
{
    [Required]
    [StringLength(80)]
    public string LotNo { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string ItemYarnType { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TotalBagQty { get; set; }

    [Range(0.001, 999999999)]
    public decimal WeightPerBag { get; set; }
}
