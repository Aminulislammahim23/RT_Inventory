using RT_Inventory.Api.DTOs.Reports;
using RT_Inventory.Api.DTOs.RfidTags;
using RT_Inventory.Api.DTOs.Stocks;

namespace RT_Inventory.Api.Interfaces;

public interface IReportService
{
    Task<IReadOnlyList<StockResponseDto>> CurrentStockAsync(string? itemType, string? lot, string? asn);
    Task<IReadOnlyList<StockResponseDto>> StockByStatusAsync(string status);
    Task<IReadOnlyList<AsnReceiveReportDto>> AsnWiseReceiveAsync(DateTime? fromDate, DateTime? toDate, string? supplier, string? lot, string? itemType, string? asn);
    Task<IReadOnlyList<RequisitionIssueReportDto>> RequisitionWiseIssueAsync(DateTime? fromDate, DateTime? toDate, string? requisition, string? rfidTag);
    Task<IReadOnlyList<RfidTagHistoryResponseDto>> RfidTagHistoryAsync(string? rfidTag);
    Task<IReadOnlyList<WarningReportDto>> WarningsAsync(DateTime? fromDate, DateTime? toDate, string? asn, string? requisition, string? rfidTag);
}
