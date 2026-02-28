using BubbleApp.Common.Entities;
using System.Threading;

namespace BubbleApp.Data.IRepository
{
    public interface IWorkspaceRepository
    {
        Task<Workspace> CreateAsync(Workspace ws, CancellationToken ct = default);
        Task<Workspace?> GetBySlugAsync(string slug, CancellationToken ct = default);
        Task<IReadOnlyList<Workspace>> ListByAdminAsync(string adminId, CancellationToken ct = default);
    }
}