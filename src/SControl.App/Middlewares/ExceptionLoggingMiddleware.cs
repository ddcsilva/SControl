using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;
using System.Diagnostics;
using SControl.App.ViewModels;

namespace SControl.App.Middlewares
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;

        public ExceptionLoggingMiddleware(RequestDelegate next, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider)
        {
            _next = next;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Log exception using your logging library
                Log.Error(ex, "Ocorreu um erro interno no servidor.");

                var model = new ErrorViewModel { RequestId = Activity.Current?.Id ?? httpContext.TraceIdentifier };
                string viewContent = await RenderViewToStringAsync(httpContext, "Views/Shared/Error.cshtml", model);

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "text/html";
                await httpContext.Response.WriteAsync(viewContent);
            }
        }

        private async Task<string> RenderViewToStringAsync(HttpContext context, string viewPath, object model)
        {
            using var writer = new StringWriter();
            var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
            var viewEngineResult = _viewEngine.GetView("~/", viewPath, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException($"Não foi possível encontrar a view: {viewPath}");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var viewContext = new ViewContext(actionContext, viewEngineResult.View, viewDictionary, new TempDataDictionary(actionContext.HttpContext, _tempDataProvider), writer, new HtmlHelperOptions());
            await viewEngineResult.View.RenderAsync(viewContext);

            return writer.ToString();
        }



    }
}
