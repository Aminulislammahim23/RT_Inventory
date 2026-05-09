namespace RT_Inventory.Api.DTOs.Returns;

public class ReturnRequestResponseDto
{
    public int Id { get; set; }
    public string ReturnNo { get; set; } = string.Empty;
    public int? SourceRequisitionId { get; set; }
    public int? ReturnAsnGrnId { get; set; }
    public string? ReturnAsnGrnNo { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
