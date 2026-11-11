using LABOFMUTANTAPI.Context;
using LABOFMUTANTAPI.Services.Interfaces;
using LABOFMUTANTAPI.Services.Services;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 262144);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Lab Of Mutant API", Version = "v1", Description = "Recepción de cambios pendientes de la PWA." });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer", Description = "Identificador secreto del dispositivo generado por la PWA (UUID). No es una cuenta de usuario." });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>() });
});
var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy("Pwa", policy => policy.WithOrigins(origins).AllowAnyHeader().WithMethods("GET", "POST")));
var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configura la conexión de SQL Server DefaultConnection.");
builder.Services.AddDbContext<AplicacionBdContext>(options => options.UseSqlServer(connection));
builder.Services.AddScoped<AplicacionDataContext>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IComentariosService, ComentariosService>();
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = context => {
    if (context.File.Name is "sw.js" or "index.html" or "evento.json")
        context.Context.Response.Headers.CacheControl = "no-cache";
} });
await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<AplicacionBdContext>();
    await database.Database.MigrateAsync();
}
if (args.Contains("--init-db")) return;
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseCors("Pwa");
app.MapGet("/health", async (AplicacionBdContext database) =>
    await database.Database.CanConnectAsync()
        ? Results.Ok(new { status = "ok", storage = "sqlserver" })
        : Results.Problem("SQL Server no está disponible.", statusCode: 503));
app.MapControllers();
app.Run();
public partial class Program { }
