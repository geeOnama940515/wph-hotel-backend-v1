using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Identity;

namespace WPHBookingSystem.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
    }
} 