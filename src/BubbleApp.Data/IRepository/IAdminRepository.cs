using BubbleApp.Common.Entities;
using System.Threading;

namespace BubbleApp.Data.IRepository
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<Admin>  CreateAsync(Admin admin, CancellationToken ct = default);
    }
}