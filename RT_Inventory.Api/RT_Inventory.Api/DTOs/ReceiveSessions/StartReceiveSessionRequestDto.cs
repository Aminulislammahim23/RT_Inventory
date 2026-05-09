using System.ComponentModel.DataAnnotations;

namespace RT_Inventory.Api.DTOs.ReceiveSessions;

public class StartReceiveSessionRequestDto
{
    [Range(1, int.MaxValue)]
    public int? AsnGrnId { get; set; }

    public string? AsnGrnNo { get; set; }
}
