using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface IProjectRepository
{
    Task<int> AddProject(Project project, int userId);
    Task<Project?> GetProjectByIdAsync(int projectId);
    Task<List<User>> GetProjectManagersAsync();
    Task<int> UpdateProjectAsync(Project project);
    Task<Pagination<User>> GetPaginatedMembersAsync(Pagination<User> pagination);
    Task<List<ProjectUser>> GetAssignedMembers(int projectId);
    Task<bool> AssignMembersAsync(int projectId, List<int>? userIds);
    Task<bool> DeleteProjectAsync(int projectId, int userId);
    Task<List<Project>> GetProjectNamesAsync(int userId, bool isAdmin);
}
