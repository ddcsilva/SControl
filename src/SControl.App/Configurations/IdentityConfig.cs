using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SControl.App.Data;

namespace SControl.App.Configurations;

public static class IdentityConfig
{
    /// <summary>
    /// Adiciona configuração do Identity à ServiceCollection.
    /// </summary>
    /// <param name="services">Collection de Serviços para adicionar a configuração do Identity.</param>
    /// <param name="configuration">Configurações do aplicativo</param>
    /// <returns>Collection de Serviços atualizada</returns>
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }
}