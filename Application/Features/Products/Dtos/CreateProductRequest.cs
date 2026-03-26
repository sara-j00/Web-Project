using System.ComponentModel.DataAnnotations;

namespace Application.Features.Products.Dtos;

public record CreateProductRequest(
    
    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(100, ErrorMessage = "Name length cannot exceed 100 characters")]
    string Name,
    
    [Required(ErrorMessage = "Description is required")]
    [MaxLength(1000, ErrorMessage = "Description length cannot exceed 1000 characters")]
    string Description,
    
    [Range(0.01, 1_000_000, ErrorMessage = "price must be more than 0")]
    decimal Price,
    
    [Range(1, int.MaxValue, ErrorMessage = "stock must be more than 0")]
    int Stock,
    
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be more than 0")]
    int CategoryId
    );
