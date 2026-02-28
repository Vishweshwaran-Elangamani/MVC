using BubbleApp.Common.ViewModels.Workspace;

namespace BubbleApp.Core.IService
{
    public interface IWorkspaceService
    {
        Task<WorkspaceDto> CreateAsync(string adminId, WorkspaceCreateRequest req, CancellationToken ct = default);
        Task<IReadOnlyList<WorkspaceDto>> ListAsync(string adminId, CancellationToken ct = default);
        Task<WorkspaceDto?> GetBySlugAsync(string slug, CancellationToken ct = default);
    }
}