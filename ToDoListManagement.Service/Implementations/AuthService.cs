using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPermissionService _permissionService;
    public AuthService(IAuthRepository authRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IPermissionService permissionService)
    {
        _authRepository = authRepository;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _permissionService = permissionService;
    }

    public async Task<bool> ValidateUserAsync(LoginViewModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return false;
        }

        bool isValid = await _authRepository.ValidateUser(model.Email, model.Password);
        User user = await _authRepository.GetUserByEmailAsync(model.Email.Trim().ToLower());

        if (isValid)
        {
            List<PermissionViewModel>? permissions = await _permissionService.GetPermissionsByRoleAsync(user.RoleId);

            string? permissionsJson = JsonSerializer.Serialize(permissions);

            List<Claim>? claims =
            [
                new Claim("Permissions", permissionsJson)
            ];
            string? token = await GenerateJwtToken(model.Email, claims, 1);
            string? refreshToken = await GenerateJwtToken(model.Email, claims, 30);

            CookieOptions? cookieToken = new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            if (model.RememberMe)
            {
                cookieToken.Expires = DateTime.Now.AddDays(30);
            }
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("Token", token, cookieToken);

            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task RefreshUserPermissionsTokenAsync(int userId)
    {
        User user = await _authRepository.GetUserByIdAsync(userId);
        if (user == null) return;

        List<PermissionViewModel> updatedPermissions = await _permissionService.GetPermissionsByRoleAsync(user.RoleId);

        string permissionsJson = JsonSerializer.Serialize(updatedPermissions);

        List<Claim>? claims =
        [
            new Claim("Permissions", permissionsJson)
        ];

        string token = await GenerateJwtToken(user.Email ?? string.Empty , claims, 1);

        CookieOptions? cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddHours(1)
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("Token", token, cookieOptions);
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _authRepository.CheckEmailExistsAsync(email.Trim().ToLower());
    }

    public async Task<string> GenerateJwtToken(string email, List<Claim> extraClaims, int expireDays)
    {
        User user = await _authRepository.GetUserByEmailAsync(email.ToLower());

        Claim[]? claims =
        [
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role?.RoleName?.ToString() ?? string.Empty)
        ];

        if (extraClaims != null && extraClaims.Count > 0)
        {
            claims = claims.Concat(extraClaims).ToArray();
        }

        string jwtKey = _configuration["JWT:Key"] ?? throw new ArgumentNullException("JWT:Key configuration is missing.");
        SymmetricSecurityKey? key = new(Encoding.UTF8.GetBytes(jwtKey));
        SigningCredentials? creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken? token = new(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(expireDays),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> RegisterUserAsync(RegisterViewModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return false;
        }
        User user = new()
        {
            Name = model.Name?.Trim(),
            Email = model.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password.Trim()),
            RoleId = 3,
            IsDeleted = false
        };

        bool isRegistered = await _authRepository.RegisterUserAsync(user);

        return isRegistered;
    }

    public void LogoutUser()
    {
        HttpContext? context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            foreach (string? cookie in context.Request.Cookies.Keys)
            {
                context.Response.Cookies.Delete(cookie);
            }
        }
    }

    public async Task<UserViewModel> GetUserFromToken(string token)
    {
        JwtSecurityTokenHandler? tokenHandler = new();
        if (tokenHandler.CanReadToken(token))
        {
            JwtSecurityToken? jwtToken = tokenHandler.ReadJwtToken(token);
            Claim? emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            string email = emailClaim?.Value ?? string.Empty;
            if (!string.IsNullOrEmpty(email))
            {
                User user = await _authRepository.GetUserByEmailAsync(email);
                if (user != null)
                {
                    UserViewModel userViewModel = new()
                    {
                        Email = email,
                        Name = user.Name,
                        UserId = user.UserId,
                        Role = user.Role?.RoleName
                    };
                    return userViewModel;
                }
            }
        }

        return new UserViewModel();
    }

    public async Task<int?> SendResetPasswordEmailAsync(string email, string resetUrl)
    {
        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(resetUrl))
        {
            return 0;
        }

        User user = await _authRepository.GetUserByEmailAsync(email.Trim().ToLower());
        if (user == null)
        {
            return -1;
        }

        string emailBody = Constants.ResetPasswordEmailBody;

        emailBody = emailBody.Replace("{ResetLink}", resetUrl);

        string subject = Constants.ResetPasswordEmailSubject;
        await SendEmailAsync(email, subject, emailBody);

        return 1;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        MimeMessage? emailToSend = new();
        emailToSend.From.Add(MailboxAddress.Parse(_configuration["MailSettings:Mail"]));
        emailToSend.To.Add(MailboxAddress.Parse(email));
        emailToSend.Subject = subject;
        emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

        using (SmtpClient? emailClient = new())
        {
            string portValue = _configuration["MailSettings:Port"] ?? throw new ArgumentNullException("MailSettings:Port configuration is missing.");
            emailClient.Connect(_configuration["MailSettings:Host"], int.Parse(portValue), SecureSocketOptions.StartTls);
            emailClient.Authenticate(_configuration["MailSettings:Mail"], _configuration["MailSettings:Password"]);
            emailClient.Send(emailToSend);
            emailClient.Disconnect(true);
        }

        await Task.CompletedTask;
    }

    public void LogError(Exception? exception)
    {
        if (exception != null)
        {
            _authRepository.LogError(exception);
        }
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        if (string.IsNullOrEmpty(model.Email))
        {
            return false;
        }

        User user = await _authRepository.GetUserByEmailAsync(model.Email.Trim().ToLower());
        if (user == null)
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword?.Trim());
        bool isUpdated = await _authRepository.UpdateUserPassword(user);

        return isUpdated;
    }
}