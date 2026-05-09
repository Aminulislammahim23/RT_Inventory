namespace RT_Inventory.Api.DTOs.Requisitions;

public class RequisitionResponseDto
{
    public int Id { get; set; }
    public string RequisitionNo { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<RequisitionItemResponseDto> Items { get; set; } = [];
    public List<PickingListResponseDto> PickingLists { get; set; } = [];
}
