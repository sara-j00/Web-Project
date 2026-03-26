using FluentValidation;

namespace Application.Features.Cart.Dtos;

public record AddToCartRequest(int ProductId, int Quantity);

public class AddToCartRequestValidator : AbstractValidator<AddToCartRequest>
{
    public AddToCartRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("ProductId must be more than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be more than 0");
    }
}