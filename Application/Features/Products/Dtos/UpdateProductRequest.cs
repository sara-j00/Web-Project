using FluentValidation;

namespace Application.Features.Products.Dtos;

public record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    int CategoryId
    );

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}