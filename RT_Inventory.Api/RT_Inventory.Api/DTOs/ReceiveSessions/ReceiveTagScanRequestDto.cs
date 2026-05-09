using System.ComponentModel.DataAnnotations;

namespace RT_Inventory.Api.DTOs.ReceiveSessions;

public class ReceiveTagScanRequestDto
{
    [Required]
    [StringLength(120)]
    public string TagValue { get; set; } = string.Empty;
}
