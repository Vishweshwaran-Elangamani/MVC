using System.Security.Claims;
using BubbleApp.Core.IService;
using BubbleApp.Common.ViewModels.Workspace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BubbleApp.Api.Controllers
{
    [ApiController]
    [Authorize] // Admin-only
    [Route("api/workspaces")]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceService _svc;
        public WorkspacesController(IWorkspaceService svc) => _svc = svc;

        private string AdminId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("Admin id missing.");

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkspaceCreateRequest req, CancellationToken ct)
            => Ok(await _svc.CreateAsync(AdminId, req, ct));

        [HttpGet]
        public async Task<IActionResult> List(CancellationToken ct)
            => Ok(await _svc.ListAsync(AdminId, ct));

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
        {
            var ws = await _svc.GetBySlugAsync(slug, ct);
            return ws is null ? NotFound() : Ok(ws);
        }
    }
}