using RT_Inventory.Api.DTOs.IssueSessions;

namespace RT_Inventory.Api.Interfaces;

public interface IIssueSessionService
{
    Task<(IssueSessionResponseDto? Session, string? Error, int StatusCode)> StartAsync(StartIssueSessionRequestDto request, int startedByUserId);
    Task<(IssueTagScanResponseDto? Scan, string? Error, int StatusCode)> ScanAsync(int sessionId, IssueTagScanRequestDto request, int performedByUserId);
    Task<(IssueSessionResponseDto? Session, string? Error, int StatusCode)> DeactivateAsync(int sessionId);
}
