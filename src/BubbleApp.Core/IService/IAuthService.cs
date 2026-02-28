using System.Threading;
using System.Threading.Tasks;
using BubbleApp.Common.ViewModels.Auth;   // <-- ensure this is present

namespace BubbleApp.Core.IService
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(AdminRegisterRequest req, CancellationToken ct = default);
        Task<AuthResponse> LoginAsync(AdminLoginRequest req, CancellationToken ct = default);
    }
}