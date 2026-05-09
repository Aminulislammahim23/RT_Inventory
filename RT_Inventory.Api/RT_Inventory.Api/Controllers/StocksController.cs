using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.DTOs.Stocks;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/stocks")]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.SCM},{RoleNames.StoreOfficer},{RoleNames.StoreSupervisor},{RoleNames.StoreManager},{RoleNames.UnitPlanner},{RoleNames.KnittingSupervisor},{RoleNames.QualityOfficer}")]
public class StocksController(IStockService stockService) : ControllerBase
{
    [HttpGet("current")]
    [ProducesResponseType(typeof(IReadOnlyList<StockResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Current([FromQuery] string? status)
    {
        return Ok(await stockService.GetCurrentAsync(status));
    }

    [HttpGet("by-item")]
    [ProducesResponseType(typeof(IReadOnlyList<StockSummaryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ByItem([FromQuery] string? itemYarnType)
    {
        return Ok(await stockService.GetByItemAsync(itemYarnType));
    }

    [HttpGet("by-lot")]
    [ProducesResponseType(typeof(IReadOnlyList<StockSummaryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ByLot([FromQuery] string? lotNo)
    {
        return Ok(await stockService.GetByLotAsync(lotNo));
    }

    [HttpGet("transactions")]
    [ProducesResponseType(typeof(IReadOnlyList<StockTransactionResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Transactions([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] string? rfidTag)
    {
        return Ok(await stockService.GetTransactionsAsync(fromDate, toDate, rfidTag));
    }
}
