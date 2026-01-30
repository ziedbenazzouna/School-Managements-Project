using AuthECAPI.Controllers;
using AuthECAPI.Extensions;
using AuthECAPI.Models;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services
    .AddSwaggerExplorer()
    .InjectDbContext(builder.Configuration)
    .AddAppConfig(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .ConfigureIdentityOptions()
    .ConfigureFileOptions()
    .AddIdentityAuth(builder.Configuration)
    ;



var app = builder.Build();

app.UseDefaultFiles(); // Searches for index.html in wwwroot
app.UseStaticFiles();

var resourcesPath = Path.Combine(builder.Environment.ContentRootPath, "Resources");
if (!Directory.Exists(resourcesPath)) Directory.CreateDirectory(resourcesPath);

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(resourcesPath),
        RequestPath = "/Resources"
    }
    );


app.ConfigureSwaggerExplorer()
    .ConfigureCors(builder.Configuration);
    
    
app.AddIdentityAuthMiddlewares();
app.MapControllers();

app
    .MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.MapGroup("/api")
    .MapIdentityUserEndpoints()
    .MapAccountEndpoints()
    .MapUploadEndpoints()
    .MapStudentDetailEndpoints()
    .MapAuthorizationDemoEndpoints();

app.MapControllers(); // Ensures API endpoints still work
app.MapFallbackToFile("index.html").AllowAnonymous(); // Redirects non-API routes to Angular

app.Run();


