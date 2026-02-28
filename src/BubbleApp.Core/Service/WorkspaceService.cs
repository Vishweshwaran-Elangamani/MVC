using System.Text.RegularExpressions;
using BubbleApp.Common.Entities;
using BubbleApp.Common.ViewModels.Workspace;
using BubbleApp.Core.IService;
using BubbleApp.Data.IRepository;

namespace BubbleApp.Core.Service
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IWorkspaceRepository _repo;
        public WorkspaceService(IWorkspaceRepository repo) => _repo = repo;

        public async Task<WorkspaceDto> CreateAsync(string adminId, WorkspaceCreateRequest req, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                throw new ArgumentException("Workspace name is required.");

            var slug = Slugify(req.Name);
            var ws = new Workspace
            {
                Id = Guid.NewGuid().ToString(),
                Name = req.Name.Trim(),
                Slug = slug,
                AdminId = adminId,
                CreatedAt = DateTime.UtcNow
            };

            ws = await _repo.CreateAsync(ws, ct);
            return new WorkspaceDto(ws.Id, ws.Name, ws.Slug, ws.CreatedAt);
        }

        public async Task<IReadOnlyList<WorkspaceDto>> ListAsync(string adminId, CancellationToken ct = default)
        {
            var list = await _repo.ListByAdminAsync(adminId, ct);
            return list.Select(w => new WorkspaceDto(w.Id, w.Name, w.Slug, w.CreatedAt)).ToList();
        }

        public async Task<WorkspaceDto?> GetBySlugAsync(string slug, CancellationToken ct = default)
        {
            var ws = await _repo.GetBySlugAsync(slug, ct);
            return ws is null ? null : new WorkspaceDto(ws.Id, ws.Name, ws.Slug, ws.CreatedAt);
        }

        private static string Slugify(string s)
        {
            s = s.ToLowerInvariant().Trim();
            s = Regex.Replace(s, @"[^a-z0-9\-]+", "-");
            s = Regex.Replace(s, "-{2,}", "-").Trim('-');
            return string.IsNullOrWhiteSpace(s) ? $"ws-{Guid.NewGuid():N}" : s;
        }
    }
}