using BubbleApp.Common.ViewModels.Notes;
using BubbleApp.Core.IService;
using Microsoft.AspNetCore.Mvc;

namespace BubbleApp.Api.Controllers
{
    [ApiController]
    [Route("api/notes")]
    public class NotesController : ControllerBase
    {
        private readonly INotesService _svc;
        public NotesController(INotesService svc) => _svc = svc;

        // GET /api/notes?workspace=eepz&userId=U123
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string workspace, [FromQuery] string userId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(workspace) || string.IsNullOrWhiteSpace(userId))
                return BadRequest("workspace and userId are required");
            var list = await _svc.GetAsync(workspace, userId, ct);
            return Ok(list);
        }

        // POST /api/notes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNoteRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Workspace) || string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.Content))
                return BadRequest("workspace, userId, content are required");
            var note = await _svc.CreateAsync(req, ct);
            return Ok(note);
        }

        // DELETE /api/notes/{id}?workspace=eepz&userId=U123
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, [FromQuery] string workspace, [FromQuery] string userId, CancellationToken ct)
        {
            var ok = await _svc.DeleteAsync(id, workspace, userId, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}