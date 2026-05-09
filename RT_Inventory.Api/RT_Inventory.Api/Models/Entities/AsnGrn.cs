using RT_Inventory.Api.Models.Enums;

namespace RT_Inventory.Api.Models.Entities;

public class AsnGrn
{
    public int Id { get; set; }
    public string AsnGrnNo { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string PoNo { get; set; } = string.Empty;
    public string ChallanNo { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public string ItemYarnType { get; set; } = string.Empty;
    public int TotalBagQty { get; set; }
    public decimal WeightPerBag { get; set; }
    public string? ChallanCopyPath { get; set; }
    public string? UsterReportPath { get; set; }
    public AsnGrnStatus Status { get; set; } = AsnGrnStatus.Parking;
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User CreatedByUser { get; set; } = null!;
    public ICollection<RfidTag> RfidTags { get; set; } = [];
    public ICollection<ReceiveSession> ReceiveSessions { get; set; } = [];
    public ICollection<Stock> Stocks { get; set; } = [];
    public MrrApproval? MrrApproval { get; set; }
}
