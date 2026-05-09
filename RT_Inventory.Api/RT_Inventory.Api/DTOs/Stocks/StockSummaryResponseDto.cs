namespace RT_Inventory.Api.DTOs.Stocks;

public class StockSummaryResponseDto
{
    public string GroupKey { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int BagQty { get; set; }
    public decimal TotalWeight { get; set; }
}
