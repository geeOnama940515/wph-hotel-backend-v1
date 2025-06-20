using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Identity;

namespace WPHBookingSystem.Infrastructure.Identity;

public interface IIdentityService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);

    Task<bool> DisableAccount(string userId);

    Task<bool> EnableAccount(string userId);

    Task<bool> AddRoleToAccount(string userId, string roleName);
    Task<bool> RemoveRoleFromAccount(string userId, string roleName);

    Task<IList<string>> GetUserRolesAsync(string userId);

    Task<List<UserResponse>> GetAllUsersAsync(); // Assuming ApplicationUser is defined in your project

    //Task<List<ApplicationUser>>
}