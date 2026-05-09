namespace RT_Inventory.Api.DTOs.Requisitions;

public class RequisitionItemResponseDto
{
    public int Id { get; set; }
    public string ItemYarnType { get; set; } = string.Empty;
    public string? LotNo { get; set; }
    public int RequiredBagQty { get; set; }
    public decimal RequiredWeight { get; set; }
}
