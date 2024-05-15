using Microsoft.OpenApi.Models;

namespace CapitalPlacement.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CapitalPlacement",
                    Version = "v1",
                    Description = "The API that powers the capital placement solution",
                    Contact = new OpenApiContact
                    {
                        Name = "Nnaemeka Eziamaka",
                        Email = "eziamakanv@gmail.com",
                    }
                });
            });
        }

    }
}
