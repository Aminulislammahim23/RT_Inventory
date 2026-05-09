using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Models.Entities;

public class Requisition
{
    public int Id { get; set; }
    public string RequisitionNo { get; set; } = string.Empty;
    public RequisitionPurpose Purpose { get; set; }
    public RequisitionStatus Status { get; set; } = RequisitionStatus.Pending;
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User CreatedByUser { get; set; } = null!;
    public ICollection<RequisitionItem> Items { get; set; } = [];
    public ICollection<PickingList> PickingLists { get; set; } = [];
}
