using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface IProjectService
{
    Task<int> AddProject(ProjectViewModel model, int userId);
    Task<ProjectViewModel?> GetProjectByIdAsync(int projectId   );
    Task<int> UpdateProjectAsync(ProjectViewModel model, int userId);
    Task<Pagination<MemberViewModel>> GetAssignedMembersAsync(Pagination<MemberViewModel> pagination, int projectId);
    Task<bool> AssignMembersAsync(int projectId, List<int>? userIds);
    Task<bool> DeleteProjectAsync(int projectId, int userId);
    Task<List<ProjectDropDown>> GetProjectNamesAsync(int userId, bool isAdmin);
}
