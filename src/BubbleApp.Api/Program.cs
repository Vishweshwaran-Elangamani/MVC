using System.Text;
using BubbleApp.Core.IService;
using BubbleApp.Core.Service;
using BubbleApp.Data.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services --------------------
builder.Services.AddControllers();

// Built-in OpenAPI document generator (no Models namespace needed)
builder.Services.AddOpenApi();

// CORS for widget & dashboard
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Widget", p => p
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BubbleApp.Api.OpenApi.BearerSecurityDocumentTransformer>();
});
// Data layer (Mongo + repositories)
builder.Services.AddDataLayer(builder.Configuration);

// Core services
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IWorkspaceService, WorkspaceService>();
builder.Services.AddSingleton<ISnippetService, SnippetService>();
builder.Services.AddSingleton<INotesService, NotesService>();

// JWT (Admin-only)
var jwtKey      = builder.Configuration["Jwt:Key"]!;
var jwtIssuer   = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var signingKey  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = jwtIssuer,
        ValidAudience            = jwtAudience,
        IssuerSigningKey         = signingKey,
        ClockSkew                = TimeSpan.FromMinutes(2)
    };
});

// -------------------- Pipeline --------------------
var app = builder.Build();

app.UseCors("Widget");

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    // Serve the OpenAPI JSON at /openapi/v1.json
    app.MapOpenApi();

    // Serve Swagger UI pointed at the built-in document (UI at /swagger)
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "Bubble API v1");
        c.DocumentTitle = "Bubble API Docs";
        c.RoutePrefix   = "swagger";
    });
}

app.MapControllers();
app.Run();