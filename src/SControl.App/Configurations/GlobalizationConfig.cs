using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace SControl.App.Configurations;

public static class GlobalizationConfig
{
    /// <summary>
    /// Configura a globalização da aplicação no pipeline de middleware especificado.
    /// </summary>
    /// <param name="app">O aplicativo para configurar a globalização.</param>
    /// <returns>O aplicativo atualizado.</returns>
    public static IApplicationBuilder UseGlobalizationConfig(this IApplicationBuilder app)
    {
        var defaultCulture = new CultureInfo("pt-BR");
        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(defaultCulture),
            SupportedCultures = new List<CultureInfo> { defaultCulture },
            SupportedUICultures = new List<CultureInfo> { defaultCulture }
        };
        app.UseRequestLocalization(localizationOptions);

        return app;
    }
}