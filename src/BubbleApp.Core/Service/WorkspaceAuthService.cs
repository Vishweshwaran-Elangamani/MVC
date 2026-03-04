using System;
using System.Threading.Tasks;
using BubbleApp.Core.IService;
using BubbleApp.Core.Security;
using BubbleApp.Data.IRepository;

namespace BubbleApp.Core.Service
{
    public sealed class WorkspaceAuthService : IWorkspaceAuthService
    {
        private readonly IWorkspaceRepository _repo;
        public WorkspaceAuthService(IWorkspaceRepository repo) => _repo = repo;

        public async Task<string> ResolveWorkspaceIdOrThrowAsync(string? slugFromClient, string? keyFromClient)
        {
            if (string.IsNullOrWhiteSpace(keyFromClient))
                throw new UnauthorizedAccessException("Missing workspace key.");

            var keyHash = WorkspaceKeyUtil.Sha256Base64Url(keyFromClient);
            var ws = await _repo.GetByKeyHashAsync(keyHash);
            if (ws is null)
                throw new UnauthorizedAccessException("Invalid workspace key.");

            if (!string.IsNullOrWhiteSpace(slugFromClient) &&
                !string.Equals(ws.Slug, slugFromClient, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Workspace mismatch.");

            return ws.Id;
        }
    }
}