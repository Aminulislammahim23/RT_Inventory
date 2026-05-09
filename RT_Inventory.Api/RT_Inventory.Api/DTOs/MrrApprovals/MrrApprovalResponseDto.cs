namespace RT_Inventory.Api.DTOs.MrrApprovals;

public class MrrApprovalResponseDto
{
    public int Id { get; set; }
    public int AsnGrnId { get; set; }
    public string AsnGrnNo { get; set; } = string.Empty;
    public string MrrNo { get; set; } = string.Empty;
    public int ApprovedByUserId { get; set; }
    public string ApprovedByUsername { get; set; } = string.Empty;
    public DateTime ApprovedAt { get; set; }
    public int ConfirmedBagQty { get; set; }
    public decimal ConfirmedWeight { get; set; }
}
