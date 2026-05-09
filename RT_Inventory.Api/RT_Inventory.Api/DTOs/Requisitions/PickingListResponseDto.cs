namespace RT_Inventory.Api.DTOs.Requisitions;

public class PickingListResponseDto
{
    public int Id { get; set; }
    public int RequisitionId { get; set; }
    public string PickingListNo { get; set; } = string.Empty;
    public string BarcodeValue { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
