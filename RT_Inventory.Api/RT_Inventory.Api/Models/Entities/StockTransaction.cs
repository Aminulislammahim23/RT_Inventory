using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Models.Entities;

public class StockTransaction
{
    public int Id { get; set; }
    public int RfidTagId { get; set; }
    public int? AsnGrnId { get; set; }
    public int? RequisitionId { get; set; }
    public StockTransactionType TransactionType { get; set; }
    public StockStatus FromStatus { get; set; }
    public StockStatus ToStatus { get; set; }
    public decimal Weight { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public RfidTag RfidTag { get; set; } = null!;
    public AsnGrn? AsnGrn { get; set; }
    public Requisition? Requisition { get; set; }
    public User CreatedByUser { get; set; } = null!;
}
