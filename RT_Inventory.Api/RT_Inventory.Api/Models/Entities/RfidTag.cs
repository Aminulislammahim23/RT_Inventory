using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Models.Entities;

public class RfidTag
{
    public int Id { get; set; }
    public string TagValue { get; set; } = string.Empty;
    public int? AsnGrnId { get; set; }
    public int? BagNumber { get; set; }
    public RfidTagAssignmentType AssignmentType { get; set; } = RfidTagAssignmentType.AsnGrnBag;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public AsnGrn? AsnGrn { get; set; }
    public ICollection<RfidTagHistory> Histories { get; set; } = [];
    public ICollection<Stock> Stocks { get; set; } = [];
    public ICollection<StockTransaction> StockTransactions { get; set; } = [];
}
