namespace RT_Inventory.Api.DTOs.Stocks;

public class StockResponseDto
{
    public int Id { get; set; }
    public int AsnGrnId { get; set; }
    public string AsnGrnNo { get; set; } = string.Empty;
    public int RfidTagId { get; set; }
    public string TagValue { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public string ItemYarnType { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
