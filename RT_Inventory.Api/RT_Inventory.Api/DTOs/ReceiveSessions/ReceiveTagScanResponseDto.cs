namespace RT_Inventory.Api.DTOs.ReceiveSessions;

public class ReceiveTagScanResponseDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ReceiveSessionId { get; set; }
    public string TagValue { get; set; } = string.Empty;
    public int? RfidTagId { get; set; }
    public int? StockId { get; set; }
    public string? WarningType { get; set; }
}
