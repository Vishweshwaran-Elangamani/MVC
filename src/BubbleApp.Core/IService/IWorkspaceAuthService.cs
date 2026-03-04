using System.Threading.Tasks;

namespace BubbleApp.Core.IService
{
    public interface IWorkspaceAuthService
    {
        Task<string> ResolveWorkspaceIdOrThrowAsync(string? slugFromClient, string? keyFromClient);
    }
}