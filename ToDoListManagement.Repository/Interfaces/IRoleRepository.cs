using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface IRoleRepository
{
    Task<List<Role>> GetAllRoles();
    Task<Role> AddRoleAsync(Role role);
    Task<Role?> GetRoleByIdAsync(int roleId);
    Task<Role?> GetRoleByNameAsync(string roleName, int roleId);
    Task<bool> UpdateRoleAsync(Role role);
}
