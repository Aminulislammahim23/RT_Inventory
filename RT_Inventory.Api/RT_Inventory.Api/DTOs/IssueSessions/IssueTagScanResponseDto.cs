namespace RT_Inventory.Api.DTOs.IssueSessions;

public class IssueTagScanResponseDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public int IssueSessionId { get; set; }
    public string TagValue { get; set; } = string.Empty;
    public int? RfidTagId { get; set; }
    public int? StockId { get; set; }
    public string? WarningType { get; set; }
}
