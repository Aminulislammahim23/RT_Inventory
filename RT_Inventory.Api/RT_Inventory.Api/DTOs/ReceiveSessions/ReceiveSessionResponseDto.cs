namespace RT_Inventory.Api.DTOs.ReceiveSessions;

public class ReceiveSessionResponseDto
{
    public int Id { get; set; }
    public int AsnGrnId { get; set; }
    public string AsnGrnNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int StartedByUserId { get; set; }
    public string StartedByUsername { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public DateTime? EndedAt { get; set; }
}
