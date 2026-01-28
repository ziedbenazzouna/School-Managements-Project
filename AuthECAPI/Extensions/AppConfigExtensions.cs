using AuthECAPI.Models;
using Microsoft.Extensions.Options;

namespace AuthECAPI.Extensions
{
    public static class AppConfigExtensions
    {
        public static WebApplication ConfigureCors(this WebApplication app, IConfiguration config)
        {
            app.UseCors(Options=>Options.WithOrigins(config["AppSettings:Client_URL"]!).AllowAnyMethod().AllowAnyHeader());
            return app;
        }

        public static IServiceCollection AddAppConfig(this IServiceCollection services, IConfiguration config)
        {
            services
           .Configure<AppSettings>(config.GetSection("AppSettings"));
            return services;
        }
    }
}
