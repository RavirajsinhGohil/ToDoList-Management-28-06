using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;

    public ProfileService(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<ProfileViewModel?> GetUserProfileAsync(string? email)
    {
        
        if (string.IsNullOrEmpty(email))
        {
            return null;
        }
        User? user = await _profileRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            return null;
        }
        return new ProfileViewModel
        {
            EmployeeId = user.UserId,
            Name = user.Name ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Role = user.RoleId
        };
    }

    public async Task<bool> UpdateUserProfileAsync(ProfileViewModel model)
    {
        User user = new()
        {
            UserId = model.EmployeeId,
            Name = model.Name,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            RoleId = model.Role
        };

        return await _profileRepository.UpdateUserAsync(user);
    }
}
