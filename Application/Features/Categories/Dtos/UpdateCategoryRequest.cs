using System.ComponentModel.DataAnnotations;

namespace Application.Features.Categories.Dtos;

public record UpdateCategoryRequest(
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(100, ErrorMessage = "Name length cannot exceed 100 characters")]
    string Name
    );
