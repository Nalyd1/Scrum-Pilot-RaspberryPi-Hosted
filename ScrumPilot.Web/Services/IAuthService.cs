using ScrumPilot.Shared.Models;

namespace ScrumPilot.Web.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginRequest request);
        Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request);
        Task LogoutAsync();
    }
}
