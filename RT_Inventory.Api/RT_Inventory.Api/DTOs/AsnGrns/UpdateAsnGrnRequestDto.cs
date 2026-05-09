using System.ComponentModel.DataAnnotations;

namespace RT_Inventory.Api.DTOs.AsnGrns;

public class UpdateAsnGrnRequestDto
{
    [Required]
    [StringLength(150)]
    public string Supplier { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string PoNo { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string ChallanNo { get; set; } = string.Empty;

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

    [StringLength(500)]
    public string? ChallanCopyPath { get; set; }

    [StringLength(500)]
    public string? UsterReportPath { get; set; }
}
