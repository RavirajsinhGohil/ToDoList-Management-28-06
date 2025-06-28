using System.Security.Claims;
using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface IAuthService
{
    Task<bool> ValidateUserAsync(LoginViewModel model);
    Task RefreshUserPermissionsTokenAsync(int userId);
    Task<bool> CheckEmailExistsAsync(string email);
    Task<string> GenerateJwtToken(string email, List<Claim> extraClaims, int expiryTime);
    Task<bool> RegisterUserAsync(RegisterViewModel model);
    void LogoutUser();
    Task<UserViewModel> GetUserFromToken(string token);
    Task<int?> SendResetPasswordEmailAsync(string email, string resetUrl);
    void LogError(Exception? exception);
    Task<bool> ResetPasswordAsync(ResetPasswordViewModel model);
}
