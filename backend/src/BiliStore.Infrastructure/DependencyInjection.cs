using BiliStore.Application.Cart;
using BiliStore.Application.Catalogue;
using BiliStore.Application.Checkout;
using BiliStore.Application.Measurements;
using BiliStore.Application.Orders;
using BiliStore.Application.Payments;
using BiliStore.Infrastructure.Cart;
using BiliStore.Infrastructure.Catalogue;
using BiliStore.Infrastructure.Checkout;
using BiliStore.Infrastructure.Data;
using BiliStore.Infrastructure.Identity;
using BiliStore.Infrastructure.Measurements;
using BiliStore.Infrastructure.Orders;
using BiliStore.Infrastructure.Payments;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BiliStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BiliStoreDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'BiliStoreDb' is not configured.");
        }

        services.AddDbContext<BiliStoreDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<BiliStoreDbContext>();

        services.AddScoped<ICartQuoteService, CartQuoteService>();
        services.AddScoped<ICatalogueService, CatalogueService>();
        services.AddScoped<ICheckoutReviewService, CheckoutReviewService>();
        services.AddScoped<IMeasurementValidationService, MeasurementValidationService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentSessionService, DevelopmentPaymentSessionService>();

        return services;
    }
}
