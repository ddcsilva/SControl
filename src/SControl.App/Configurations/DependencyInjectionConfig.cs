using Microsoft.AspNetCore.Mvc.DataAnnotations;
using SControl.App.Extensions;
using SControl.Business.Intefaces;
using SControl.Business.Notificacoes;
using SControl.Business.Services;
using SControl.Data.Context;
using SControl.Data.Repository;

namespace SControl.App.Configurations;

public static class DependencyInjectionConfig
{
    /// <summary>
    /// Resolve as dependências da aplicação e adicioná-las à coleção de serviços especificada.
    /// </summary>
    /// <param name="services">A coleção de serviços para adicionar as dependências resolvidas.</param>
    /// <returns>A coleção de serviços atualizada.</returns>
    public static IServiceCollection ResolveDependencies(this IServiceCollection services)
    {
        services.AddScoped<MeuDbContext>();
        services.AddScoped<ICursoRepository, CursoRepository>();

        services.AddScoped<INotificador, Notificador>();
        services.AddScoped<ICursoService, CursoService>();

        return services;
    }
}