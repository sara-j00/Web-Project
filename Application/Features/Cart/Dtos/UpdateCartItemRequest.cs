using System.ComponentModel.DataAnnotations;

namespace Application.Features.Cart.Dtos;
public record UpdateCartItemRequest(
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be more than 0")]
    int Quantity
    );
