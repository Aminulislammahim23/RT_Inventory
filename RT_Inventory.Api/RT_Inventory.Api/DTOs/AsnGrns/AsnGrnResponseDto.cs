namespace RT_Inventory.Api.DTOs.AsnGrns;

public class AsnGrnResponseDto
{
    public int Id { get; set; }
    public string AsnGrnNo { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string PoNo { get; set; } = string.Empty;
    public string ChallanNo { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public string ItemYarnType { get; set; } = string.Empty;
    public int TotalBagQty { get; set; }
    public decimal WeightPerBag { get; set; }
    public decimal TotalWeight { get; set; }
    public string? ChallanCopyPath { get; set; }
    public string? UsterReportPath { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
