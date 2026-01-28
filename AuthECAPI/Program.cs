using AuthECAPI.Controllers;
using AuthECAPI.Extensions;
using AuthECAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services
    .AddSwaggerExplorer()
    .InjectDbContext(builder.Configuration)
    .AddAppConfig(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .ConfigureIdentityOptions()                
    .AddIdentityAuth(builder.Configuration)
    ;



var app = builder.Build();

app.UseDefaultFiles(); // Searches for index.html in wwwroot
app.UseStaticFiles();


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
    .MapStudentDetailEndpoints()
    .MapAuthorizationDemoEndpoints();

app.MapControllers(); // Ensures API endpoints still work
app.MapFallbackToFile("index.html").AllowAnonymous(); // Redirects non-API routes to Angular

app.Run();


