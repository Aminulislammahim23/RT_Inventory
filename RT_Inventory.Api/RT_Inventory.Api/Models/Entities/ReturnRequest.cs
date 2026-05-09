using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Models.Entities;

public class ReturnRequest
{
    public int Id { get; set; }
    public string ReturnNo { get; set; } = string.Empty;
    public int? SourceRequisitionId { get; set; }
    public int? ReturnAsnGrnId { get; set; }
    public ReturnRequestStatus Status { get; set; } = ReturnRequestStatus.Pending;
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Requisition? SourceRequisition { get; set; }
    public AsnGrn? ReturnAsnGrn { get; set; }
    public User CreatedByUser { get; set; } = null!;
}
