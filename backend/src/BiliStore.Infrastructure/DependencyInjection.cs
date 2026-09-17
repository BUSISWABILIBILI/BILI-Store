using BiliStore.Application.Catalogue;
using BiliStore.Application.Measurements;
using BiliStore.Infrastructure.Catalogue;
using BiliStore.Infrastructure.Data;
using BiliStore.Infrastructure.Identity;
using BiliStore.Infrastructure.Measurements;
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

        services.AddScoped<ICatalogueService, CatalogueService>();
        services.AddScoped<IMeasurementValidationService, MeasurementValidationService>();

        return services;
    }
}
