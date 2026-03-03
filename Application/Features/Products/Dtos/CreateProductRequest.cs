using FluentValidation;

namespace Application.Features.Products.Dtos;

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int CategoryId
    );

public class CreateProductRequestValidator
    : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150); 
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThan(0).LessThan(1_000_000);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}