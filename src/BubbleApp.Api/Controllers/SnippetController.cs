using BubbleApp.Core.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BubbleApp.Api.Controllers
{
    [ApiController]
    [Authorize] // Admin-only
    [Route("api/snippet")]
    public class SnippetController : ControllerBase
    {
        private readonly ISnippetService _svc;
        private readonly IConfiguration _cfg;

        public SnippetController(ISnippetService svc, IConfiguration cfg)
        {
            _svc = svc;
            _cfg = cfg;
        }

        // GET /api/snippet/{workspaceSlug}
        [HttpGet("{workspaceSlug}")]
        public IActionResult Get(string workspaceSlug)
        {
            var cdn = _cfg["Widget:CdnUrl"]
                ?? throw new InvalidOperationException("Widget:CdnUrl missing in appsettings.json");
            var resp = _svc.Generate(workspaceSlug, new Uri(cdn));
            return Ok(resp);
        }
    }
}