namespace RT_Inventory.Api.DTOs.Reports;

public class WarningReportDto
{
    public int Id { get; set; }
    public string WarningType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RfidTagValue { get; set; }
    public int? AsnGrnId { get; set; }
    public int? RequisitionId { get; set; }
    public int? ReceiveSessionId { get; set; }
    public int? IssueSessionId { get; set; }
    public DateTime CreatedAt { get; set; }
}
