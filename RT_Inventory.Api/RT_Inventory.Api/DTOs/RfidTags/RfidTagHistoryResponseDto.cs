namespace RT_Inventory.Api.DTOs.RfidTags;

public class RfidTagHistoryResponseDto
{
    public int Id { get; set; }
    public int RfidTagId { get; set; }
    public string TagValue { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public int? PerformedByUserId { get; set; }
    public string? PerformedByUsername { get; set; }
    public DateTime CreatedAt { get; set; }
}
