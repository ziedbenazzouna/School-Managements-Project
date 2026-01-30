

using Microsoft.AspNetCore.Mvc;

namespace AuthECAPI.Controllers
{
    public static class UploadEndpoints
    {
        public static IEndpointRouteBuilder MapUploadEndpoints(this IEndpointRouteBuilder app)
        {
            // Map the POST endpoint
            app.MapPost("/upload", UploadImage).DisableAntiforgery();
            return app;
        }
        [DisableRequestSizeLimit]
        private static async Task<IResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Results.BadRequest("No file uploaded.");

                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                // Ensure directory exists
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);

                // Use a unique name to prevent overwriting existing files
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Results.Ok(new { dbPath });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Internal server error: {ex.Message}");
            }
        }
    }
}
