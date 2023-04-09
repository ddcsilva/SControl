using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SControl.App.Configurations;
using SControl.App.Middlewares;
using SControl.Data.Context;
using Serilog;

// Cria um construtor de aplicação Web
var builder = WebApplication.CreateBuilder(args);

// Configura o Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configura a aplicação usando arquivos de configuração e variáveis de ambiente
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

// Adiciona a configuração do identity à aplicação
builder.Services.AddIdentityConfiguration(builder.Configuration);

// Adiciona o contexto do banco de dados com a DefaultConnection
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona o AutoMapper usando assemblies do domínio atual
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Adiciona configurações do MVC
builder.Services.AddMvcConfiguration();

// Resolve as dependências
builder.Services.ResolveDependencies();

// Constrói a aplicação
var app = builder.Build();

// Configura o tratamento de exceções com base no ambiente
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.Use(async (context, next) =>
    {
        var viewEngine = app.Services.GetRequiredService<ICompositeViewEngine>();
        var tempDataProvider = app.Services.GetRequiredService<ITempDataProvider>();
        var middleware = new ExceptionLoggingMiddleware(next, viewEngine, tempDataProvider);
        await middleware.InvokeAsync(context);
    });
    app.UseExceptionHandler("/erro/500");
    app.UseStatusCodePagesWithRedirects("/erro/{0}");
    app.UseHsts();
}

// Redireciona para HTTPS
app.UseHttpsRedirection();

// Habilita arquivos estáticos
app.UseStaticFiles();

// Adiciona suporte a roteamento
app.UseRouting();

// Adiciona suporte a autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Configura a globalização da aplicação
app.UseGlobalizationConfig();

// Mapeia a rota padrão dos controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapeia as páginas Razor
app.MapRazorPages();

// Inicia a aplicação
app.Run();
