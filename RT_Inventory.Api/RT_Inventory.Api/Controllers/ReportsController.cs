using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.SCM},{RoleNames.StoreOfficer},{RoleNames.StoreSupervisor},{RoleNames.StoreManager},{RoleNames.UnitPlanner},{RoleNames.KnittingSupervisor},{RoleNames.QualityOfficer}")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("current-stock")]
    public async Task<IActionResult> CurrentStock([FromQuery] string? itemType, [FromQuery] string? lot, [FromQuery] string? asn) => Ok(await reportService.CurrentStockAsync(itemType, lot, asn));

    [HttpGet("parking-stock")]
    public async Task<IActionResult> ParkingStock() => Ok(await reportService.StockByStatusAsync("Parking"));

    [HttpGet("confirmed-stock")]
    public async Task<IActionResult> ConfirmedStock() => Ok(await reportService.StockByStatusAsync("Confirmed"));

    [HttpGet("issued-stock")]
    public async Task<IActionResult> IssuedStock() => Ok(await reportService.StockByStatusAsync("Issued"));

    [HttpGet("asn-wise-receive")]
    public async Task<IActionResult> AsnWiseReceive([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] string? supplier, [FromQuery] string? lot, [FromQuery] string? itemType, [FromQuery] string? asn)
        => Ok(await reportService.AsnWiseReceiveAsync(fromDate, toDate, supplier, lot, itemType, asn));

    [HttpGet("requisition-wise-issue")]
    public async Task<IActionResult> RequisitionWiseIssue([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] string? requisition, [FromQuery] string? rfidTag)
        => Ok(await reportService.RequisitionWiseIssueAsync(fromDate, toDate, requisition, rfidTag));

    [HttpGet("rfid-tag-history")]
    public async Task<IActionResult> RfidTagHistory([FromQuery] string? rfidTag) => Ok(await reportService.RfidTagHistoryAsync(rfidTag));

    [HttpGet("warnings")]
    public async Task<IActionResult> Warnings([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] string? asn, [FromQuery] string? requisition, [FromQuery] string? rfidTag)
        => Ok(await reportService.WarningsAsync(fromDate, toDate, asn, requisition, rfidTag));
}
