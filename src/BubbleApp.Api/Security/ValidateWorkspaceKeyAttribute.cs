using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BubbleApp.Core.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BubbleApp.Api.Security
{
    /// <summary>
    /// Ensures requests provide a valid workspace key.
    /// Looks in this order: ActionArguments (bound DTO) -> Query -> JSON Body.
    /// On success, stores WorkspaceId in HttpContext.Items["WorkspaceId"].
    /// </summary>
    public sealed class ValidateWorkspaceKeyAttribute : TypeFilterAttribute
    {
        public ValidateWorkspaceKeyAttribute() : base(typeof(Filter)) { }

        private sealed class Filter : IAsyncActionFilter
        {
            private readonly IWorkspaceAuthService _auth;
            public Filter(IWorkspaceAuthService auth) => _auth = auth;

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var http = context.HttpContext;
                var req  = http.Request;

                string? slug = null;
                string? key  = null;

                // 1) Try bound arguments (works after MVC model binding)
                if (context.ActionArguments.Count > 0)
                {
                    foreach (var arg in context.ActionArguments.Values)
                    {
                        if (arg is null) continue;

                        // Try to find properties named "workspace" and "key" on the DTO
                        var t = arg.GetType();
                        var pWorkspace = t.GetProperty("workspace") ?? t.GetProperty("Workspace");
                        var pKey       = t.GetProperty("key")       ?? t.GetProperty("Key");

                        if (pWorkspace != null && slug is null)
                            slug = pWorkspace.GetValue(arg)?.ToString();

                        if (pKey != null && key is null)
                            key = pKey.GetValue(arg)?.ToString();
                    }
                }

                // 2) Query string fallback
                if (string.IsNullOrWhiteSpace(slug))
                    slug = req.Query["workspace"];
                if (string.IsNullOrWhiteSpace(key))
                    key  = req.Query["key"];

                // 3) JSON body fallback (only if still missing)
                if (string.IsNullOrWhiteSpace(key) &&
                    !string.IsNullOrWhiteSpace(req.ContentType) &&
                    req.ContentType.Contains("application/json"))
                {
                    // Allow re-read in case body was already consumed
                    req.EnableBuffering();

                    using var reader = new StreamReader(req.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    req.Body.Position = 0; // reset for downstream

                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        try
                        {
                            using var doc = JsonDocument.Parse(body);
                            var root = doc.RootElement;

                            if (string.IsNullOrWhiteSpace(slug) && root.TryGetProperty("workspace", out var wProp))
                                slug = wProp.GetString();

                            if (string.IsNullOrWhiteSpace(key) && root.TryGetProperty("key", out var kProp))
                                key = kProp.GetString();
                        }
                        catch
                        {
                            // ignore parse errors; validation will handle missing key.
                        }
                    }
                }

                try
                {
                    var workspaceId = await _auth.ResolveWorkspaceIdOrThrowAsync(slug, key);
                    http.Items["WorkspaceId"] = workspaceId;
                    await next();
                }
                catch (UnauthorizedAccessException ex)
                {
                    context.Result = new ObjectResult(new { error = ex.Message })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }
            }
        }
    }
}