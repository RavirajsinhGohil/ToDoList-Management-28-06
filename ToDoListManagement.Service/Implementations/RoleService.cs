using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionService _permissionService;
    private readonly IAuthService _authService;
    public RoleService(IRoleRepository roleRepository, IPermissionService permissionService, IAuthService authService)
    {
        _roleRepository = roleRepository;
        _permissionService = permissionService;
        _authService = authService;
    }

    public async Task<List<RoleViewModel>> GetAllRoles()
    {
        List<Role> roles = await _roleRepository.GetAllRoles();

        List<RoleViewModel> roleViewModels = [];
        foreach (Role? role in roles)
        {
            roleViewModels.Add(new()
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            });
        }
        return roleViewModels;
    }

    public async Task<RoleViewModel> AddRoleAsync(RoleViewModel model, int userId)
    {
        Role role = new()
        {
            RoleName = model.RoleName?.Trim(),
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        Role roleWithId = await _roleRepository.AddRoleAsync(role);
        RoleViewModel roleViewModel = new(){
            RoleId = roleWithId.RoleId,
            RoleName = roleWithId.RoleName
        };
        if(roleWithId != null)
        {
            await _permissionService.AddPermissionsAsync(roleWithId.RoleId, userId);
        }
        return roleViewModel;
    }
    
    public async Task<bool> CheckRoleNameExistsAsync(string roleName, int roleId = 0)
    {
        Role? role = await _roleRepository.GetRoleByNameAsync(roleName.Trim().ToLower(), roleId);
        if(role != null)
        {
            return true;
        }
        return false;
    }


    public async Task<RoleViewModel> GetRoleByIdAsync(int roleId)
    {
        Role? role = await _roleRepository.GetRoleByIdAsync(roleId);

        if (role == null)
        {
            return new RoleViewModel();
        }

        return new RoleViewModel()
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName
        };
    }

    public async Task<bool> UpdateRoleAsync(RoleViewModel model, int userId)
    {
        Role? role = await _roleRepository.GetRoleByIdAsync(model.RoleId);

        if (role != null)
        {
            role.RoleName = model.RoleName?.Trim();
            role.UpdatedAt = DateTime.UtcNow;
            role.UpdatedBy = userId;
        }
        else
        {
            return false;
        }

        return await _roleRepository.UpdateRoleAsync(role);
    }

    public async Task<bool> DeleteRoleAsync(int roleId, int userId)
    {
        Role? role = await _roleRepository.GetRoleByIdAsync(roleId);

        if (role != null)
        {
            role.IsDeleted = true;
            role.UpdatedAt = DateTime.UtcNow;
            role.UpdatedBy = userId;
        }
        else
        {
            return false;
        }

        return await _roleRepository.UpdateRoleAsync(role);
    }
}
