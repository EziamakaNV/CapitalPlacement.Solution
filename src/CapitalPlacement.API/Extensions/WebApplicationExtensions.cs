using CapitalPlacement.API.Middleware;

namespace CapitalPlacement.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
