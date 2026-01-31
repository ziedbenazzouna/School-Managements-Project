using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;

namespace AuthECAPI.Extensions
{
    public static class FileExtensions
    {
        public static WebApplication ConfigureFile(this WebApplication app, IConfiguration config)
        {
            app.UseDefaultFiles(); // Searches for index.html in wwwroot
            app.UseStaticFiles();

            var resourcesPath = Path.Combine(app.Environment.ContentRootPath, "Resources");
            if (!Directory.Exists(resourcesPath)) Directory.CreateDirectory(resourcesPath);

            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(resourcesPath),
                    RequestPath = "/Resources"
                }
                );
            return app;
        }
        public static IServiceCollection ConfigureFileOptions(this IServiceCollection services)
        {
            // Configure large file limits if necessary
            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
            return services;
        }
    }
}
