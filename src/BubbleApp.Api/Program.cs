using System.Text;
using BubbleApp.Core.IService;
using BubbleApp.Core.Service;
using BubbleApp.Data.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using BubbleApp.Core.IService;
using BubbleApp.Core.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS, Data layer, Core services (existing)
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(opt => { opt.AddPolicy("Widget", p => p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()); });
builder.Services.AddDataLayer(builder.Configuration);
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IWorkspaceService, WorkspaceService>();
builder.Services.AddSingleton<ISnippetService, SnippetService>();
builder.Services.AddSingleton<INotesService, NotesService>();

// after other service registrations:
builder.Services.AddScoped<IWorkspaceAuthService, WorkspaceAuthService>(); // NEW
// ...
// NEW: workspace key validation service
builder.Services.AddScoped<IWorkspaceAuthService, WorkspaceAuthService>();

// JWT auth (unchanged)
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

var app = builder.Build();
app.UseCors("Widget");
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "Bubble API v1");
        c.DocumentTitle = "Bubble API Docs";
        c.RoutePrefix = "swagger";
    });
}
app.MapControllers();
app.Run();