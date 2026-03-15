using Application.Abstraction;

namespace Infrastructure.Services;

public class MockPaymentService : IPaymentService
{
    public Task<bool> ProcessPaymentAsync(decimal amount)
    {
        // Always succeed (or fail based on amount for testing)
        return Task.FromResult(true);
    }
}