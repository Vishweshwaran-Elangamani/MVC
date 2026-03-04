using System.Threading;
using System.Threading.Tasks;
using BubbleApp.Api.RealTime;
using BubbleApp.Api.Security;
using BubbleApp.Data.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BubbleApp.Api.Controllers
{
    [ApiController]
    [Route("api/widget/config")]
    [ValidateWorkspaceKey] // validates workspace+key, sets WorkspaceId in HttpContext.Items
    public class WidgetConfigController : ControllerBase
    {
        private readonly IWorkspaceRepository _repo;
        public WidgetConfigController(IWorkspaceRepository repo) => _repo = repo;

        // Short config (initial load)
        // GET /api/widget/config?workspace=...&key=...
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string workspace, CancellationToken ct)
        {
            var ws = await _repo.GetBySlugAsync(workspace, ct);
            if (ws is null) return NotFound();

            SetNoCache();
            return Ok(new
            {
                color = string.IsNullOrWhiteSpace(ws.BubbleColor) ? "#5b8def" : ws.BubbleColor,
                text  = string.IsNullOrWhiteSpace(ws.BubbleText)  ? "●"       : ws.BubbleText,
                version = AppearanceChangeBus.Current(ws.Id)
            });
        }

        // Long-poll (return when changed or after 25s)
        // GET /api/widget/config/long?workspace=...&key=...&since=<version>
        [HttpGet("long")]
        public async Task<IActionResult> Long([FromQuery] string workspace, [FromQuery] long since = 0, CancellationToken ct = default)
        {
            var workspaceId = HttpContext.Items["WorkspaceId"] as string;
            if (string.IsNullOrWhiteSpace(workspaceId)) return Unauthorized();

            var version = await AppearanceChangeBus.WaitForChangeAsync(workspaceId, since, TimeSpan.FromSeconds(25), ct);

            var ws = await _repo.GetByIdAsync(workspaceId, ct);
            if (ws is null) return NotFound();

            SetNoCache();

            if (version == since)
                return NoContent(); // no change within timeout; client loops immediately

            return Ok(new
            {
                color = string.IsNullOrWhiteSpace(ws.BubbleColor) ? "#5b8def" : ws.BubbleColor,
                text  = string.IsNullOrWhiteSpace(ws.BubbleText)  ? "●"       : ws.BubbleText,
                version
            });
        }

        private void SetNoCache()
        {
            Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
            Response.Headers.Pragma = "no-cache";
            Response.Headers.Expires = "0";
        }
    }
}