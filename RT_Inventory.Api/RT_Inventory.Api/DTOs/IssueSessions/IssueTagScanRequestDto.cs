using System.ComponentModel.DataAnnotations;

namespace RT_Inventory.Api.DTOs.IssueSessions;

public class IssueTagScanRequestDto
{
    [Required]
    [StringLength(120)]
    public string TagValue { get; set; } = string.Empty;
}
