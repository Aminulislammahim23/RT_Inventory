namespace RT_Inventory.Api.Models.Entities;

public class RfidTagHistory
{
    public int Id { get; set; }
    public int RfidTagId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public int? PerformedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public RfidTag RfidTag { get; set; } = null!;
    public User? PerformedByUser { get; set; }
}
