using System.ComponentModel.DataAnnotations;

namespace RT_Inventory.Api.DTOs.IssueSessions;

public class StartIssueSessionRequestDto
{
    [Range(1, int.MaxValue)]
    public int? PickingListId { get; set; }

    public string? PickingListNo { get; set; }
}
