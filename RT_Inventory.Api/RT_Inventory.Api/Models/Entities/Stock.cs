using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Models.Entities;

public class Stock
{
    public int Id { get; set; }
    public int AsnGrnId { get; set; }
    public int RfidTagId { get; set; }
    public string LotNo { get; set; } = string.Empty;
    public string ItemYarnType { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public StockStatus Status { get; set; } = StockStatus.Parking;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public AsnGrn AsnGrn { get; set; } = null!;
    public RfidTag RfidTag { get; set; } = null!;
}
