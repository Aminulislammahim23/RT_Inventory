namespace RT_Inventory.Api.DTOs.RfidTags;

public class RfidTagResponseDto
{
    public int Id { get; set; }
    public string TagValue { get; set; } = string.Empty;
    public int? AsnGrnId { get; set; }
    public string? AsnGrnNo { get; set; }
    public int? BagNumber { get; set; }
    public string AssignmentType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
