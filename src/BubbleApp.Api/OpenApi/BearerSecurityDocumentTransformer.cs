using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace BubbleApp.Api.OpenApi;

public sealed class BearerSecurityDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        // Define the Bearer scheme
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme."
        };

        // Apply the requirement to all operations
        foreach (var path in document.Paths.Values)
        {
            foreach (var op in path.Operations.Values)
            {
                op.Security ??= new List<OpenApiSecurityRequirement>();
                op.Security.Add(new OpenApiSecurityRequirement
                {
                    [ new OpenApiSecurityScheme
                        { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }
                    ] = Array.Empty<string>()
                });
            }
        }

        return Task.CompletedTask;
    }
}