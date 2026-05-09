using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Models.Entities;

public class WarningLog
{
    public int Id { get; set; }
    public WarningType WarningType { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? RfidTagValue { get; set; }
    public int? AsnGrnId { get; set; }
    public int? RequisitionId { get; set; }
    public int? ReceiveSessionId { get; set; }
    public int? IssueSessionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AsnGrn? AsnGrn { get; set; }
    public Requisition? Requisition { get; set; }
    public ReceiveSession? ReceiveSession { get; set; }
    public IssueSession? IssueSession { get; set; }
}
