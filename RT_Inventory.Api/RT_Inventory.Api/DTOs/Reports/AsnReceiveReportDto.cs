namespace RT_Inventory.Api.DTOs.Reports;

public class AsnReceiveReportDto
{
    public int AsnGrnId { get; set; }
    public string AsnGrnNo { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public string ItemYarnType { get; set; } = string.Empty;
    public int ReceivedBagQty { get; set; }
    public decimal ReceivedWeight { get; set; }
}
