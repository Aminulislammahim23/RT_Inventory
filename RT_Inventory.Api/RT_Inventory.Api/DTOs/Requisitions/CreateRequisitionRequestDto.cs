using System.ComponentModel.DataAnnotations;

namespace RT_Inventory.Api.DTOs.Requisitions;

public class CreateRequisitionRequestDto
{
    [Required]
    public string Purpose { get; set; } = string.Empty;

    [MinLength(1)]
    public List<CreateRequisitionItemRequestDto> Items { get; set; } = [];
}
