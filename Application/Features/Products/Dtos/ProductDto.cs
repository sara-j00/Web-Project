namespace Application.Features.Products.Dtos;

public record ProductDto (
        int Id,
        string Name,
        string Description,
        decimal Price,
        int Stock,
        int CategoryId,
        List<string> Images
    );
