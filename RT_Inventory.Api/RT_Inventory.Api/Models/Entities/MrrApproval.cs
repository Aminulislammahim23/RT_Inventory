namespace RT_Inventory.Api.Models.Entities;

public class MrrApproval
{
    public int Id { get; set; }
    public int AsnGrnId { get; set; }
    public string MrrNo { get; set; } = string.Empty;
    public int ApprovedByUserId { get; set; }
    public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;

    public AsnGrn AsnGrn { get; set; } = null!;
    public User ApprovedByUser { get; set; } = null!;
}
