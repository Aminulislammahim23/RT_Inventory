namespace RT_Inventory.Api.Models.Entities;

public class PickingList
{
    public int Id { get; set; }
    public int RequisitionId { get; set; }
    public string PickingListNo { get; set; } = string.Empty;
    public string BarcodeValue { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Requisition Requisition { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    public ICollection<IssueSession> IssueSessions { get; set; } = [];
}
