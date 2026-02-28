using BubbleApp.Common.Entities;
using BubbleApp.Common.ViewModels.Notes;
using System.Threading;

namespace BubbleApp.Data.IRepository
{
    public interface INoteRepository
    {
        Task<IReadOnlyList<NoteDto>> ListAsync(string workspace, string userId, CancellationToken ct = default);
        Task<NoteDto> CreateAsync(Note n, CancellationToken ct = default);
        Task<bool>    DeleteAsync(string id, string workspace, string userId, CancellationToken ct = default);
    }
}