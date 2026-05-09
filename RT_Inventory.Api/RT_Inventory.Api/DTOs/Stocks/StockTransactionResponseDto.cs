namespace RT_Inventory.Api.DTOs.Stocks;

public class StockTransactionResponseDto
{
    public int Id { get; set; }
    public int RfidTagId { get; set; }
    public string TagValue { get; set; } = string.Empty;
    public int? AsnGrnId { get; set; }
    public string? AsnGrnNo { get; set; }
    public int? RequisitionId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public int CreatedByUserId { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
