using RT_Inventory.Api.DTOs.ReceiveSessions;

namespace RT_Inventory.Api.Interfaces;

public interface IReceiveSessionService
{
    Task<(ReceiveSessionResponseDto? Session, string? Error, int StatusCode)> StartAsync(StartReceiveSessionRequestDto request, int startedByUserId);
    Task<(ReceiveTagScanResponseDto? Scan, string? Error, int StatusCode)> ScanAsync(int sessionId, ReceiveTagScanRequestDto request, int performedByUserId);
    Task<(ReceiveSessionResponseDto? Session, string? Error, int StatusCode)> DeactivateAsync(int sessionId);
}
