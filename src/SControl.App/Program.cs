using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SControl.App.Configurations;
using SControl.App.Middlewares;
using SControl.Data.Context;
using Serilog;

// Cria um construtor de aplica��o Web
var builder = WebApplication.CreateBuilder(args);

// Configura o Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configura a aplica��o usando arquivos de configura��o e vari�veis de ambiente
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

// Adiciona a configura��o do identity � aplica��o
builder.Services.AddIdentityConfiguration(builder.Configuration);

// Adiciona o contexto do banco de dados com a DefaultConnection
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona o AutoMapper usando assemblies do dom�nio atual
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Adiciona configura��es do MVC
builder.Services.AddMvcConfiguration();

// Resolve as depend�ncias
builder.Services.ResolveDependencies();

// Constr�i a aplica��o
var app = builder.Build();

// Configura o tratamento de exce��es com base no ambiente
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

// Habilita arquivos est�ticos
app.UseStaticFiles();

// Adiciona suporte a roteamento
app.UseRouting();

// Adiciona suporte a autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();

// Configura a globaliza��o da aplica��o
app.UseGlobalizationConfig();

// Mapeia a rota padr�o dos controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapeia as p�ginas Razor
app.MapRazorPages();

// Inicia a aplica��o
app.Run();
