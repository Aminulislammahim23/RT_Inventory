using RT_Inventory.Api.DTOs.Stocks;

namespace RT_Inventory.Api.Interfaces;

public interface IStockService
{
    Task<IReadOnlyList<StockResponseDto>> GetCurrentAsync(string? status);
    Task<IReadOnlyList<StockSummaryResponseDto>> GetByItemAsync(string? itemYarnType);
    Task<IReadOnlyList<StockSummaryResponseDto>> GetByLotAsync(string? lotNo);
    Task<IReadOnlyList<StockTransactionResponseDto>> GetTransactionsAsync(DateTime? fromDate, DateTime? toDate, string? rfidTag);
}
