using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Models.Entities;

public class IssueSession
{
    public int Id { get; set; }
    public int PickingListId { get; set; }
    public GateSessionStatus Status { get; set; } = GateSessionStatus.Active;
    public int StartedByUserId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    public PickingList PickingList { get; set; } = null!;
    public User StartedByUser { get; set; } = null!;
    public ICollection<WarningLog> WarningLogs { get; set; } = [];
}
