namespace RT_Inventory.Api.Models.Entities;

public class RequisitionItem
{
    public int Id { get; set; }
    public int RequisitionId { get; set; }
    public string ItemYarnType { get; set; } = string.Empty;
    public string? LotNo { get; set; }
    public int RequiredBagQty { get; set; }
    public decimal RequiredWeight { get; set; }

    public Requisition Requisition { get; set; } = null!;
}
