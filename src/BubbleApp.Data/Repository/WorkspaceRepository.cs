using BubbleApp.Common.Entities;
using BubbleApp.Data.IRepository;
using BubbleApp.Data.Mongo;
using MongoDB.Driver;
using System.Threading;

namespace BubbleApp.Data.Repository
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly MongoContext _ctx;
        public WorkspaceRepository(MongoContext ctx) => _ctx = ctx;

        public async Task<Workspace> CreateAsync(Workspace ws, CancellationToken ct = default)
        {
            await _ctx.Workspaces.InsertOneAsync(ws, cancellationToken: ct);
            return ws;
        }

        public Task<Workspace?> GetBySlugAsync(string slug, CancellationToken ct = default)
            => _ctx.Workspaces.Find(w => w.Slug == slug).FirstOrDefaultAsync(ct)!;

        public async Task<IReadOnlyList<Workspace>> ListByAdminAsync(string adminId, CancellationToken ct = default)
        {
            var list = await _ctx.Workspaces.Find(w => w.AdminId == adminId).ToListAsync(ct);
            return list;
        }
    }
}