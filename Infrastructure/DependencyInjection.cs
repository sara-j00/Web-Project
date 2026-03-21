using Application.Abstraction;
using Application.Abstractions;
using Application.Features.Admin.Services;
using Application.Features.Auth.Services;
using Application.Features.Cart.Services;
using Application.Features.Categories.Services;
using Application.Features.Orders.Services;
using Application.Features.Products.Services;
using Application.Features.Wishlist.Services;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.UnitOfWork;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        // Configure DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Configure Identity
        services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();

        // Unit of Work / Infrastructure
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IImageStorageService, LocalImageStorageService>();
        services.AddScoped<IEmailService, SendGridEmailService>();
        services.AddScoped<IEmailService, ResendEmailService>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, MockPaymentService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }
}
