using CapitalPlacement.API.Features.Program;
using CapitalPlacement.API.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.OpenApi.Models;
using System.Reflection;

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

        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<UpdateProgram.Command, EmployerProgram>
                .NewConfig()
                .Map(dest => dest.id, src => src.programid);
        }
    }
}
