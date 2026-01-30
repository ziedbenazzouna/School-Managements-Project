using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;

namespace AuthECAPI.Extensions
{
    public static class FileExtensions
    {
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
