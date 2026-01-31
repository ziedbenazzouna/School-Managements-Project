using AuthECAPI.Controllers;
using AuthECAPI.Extensions;
using AuthECAPI.Models;
using Microsoft.Extensions.FileProviders;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;
QuestPDF.Settings.UseEnvironmentFonts = false;

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

app.ConfigureFile(builder.Configuration);
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


