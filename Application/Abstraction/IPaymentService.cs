namespace Application.Abstraction;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(decimal amount);
}