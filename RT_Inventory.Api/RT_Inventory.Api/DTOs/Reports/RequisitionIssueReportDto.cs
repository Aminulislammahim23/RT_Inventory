namespace RT_Inventory.Api.DTOs.Reports;

public class RequisitionIssueReportDto
{
    public int RequisitionId { get; set; }
    public string RequisitionNo { get; set; } = string.Empty;
    public int IssuedBagQty { get; set; }
    public decimal IssuedWeight { get; set; }
}
