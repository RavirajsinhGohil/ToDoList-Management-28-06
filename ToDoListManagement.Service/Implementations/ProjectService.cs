using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<int> AddProject(ProjectViewModel model, int userId)
    {
        TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        Project project = new()
        {
            ProjectName = model.ProjectName?.Trim(),
            CreatedBy = userId,
            Description = model.Description,
            Status = "Active",
            AssignedToPM = model.AssignedToPM,
            IsDeleted = false,
            CreatedOn = TimeZoneInfo.ConvertTimeToUtc(model.StartDate ?? DateTime.UtcNow, localZone),
            EndDate = TimeZoneInfo.ConvertTimeToUtc(model.EndDate ?? DateTime.UtcNow, localZone)
        };

        return await _projectRepository.AddProject(project, model.AssignedToPM ?? 0);
    }

    public async Task<ProjectViewModel?> GetProjectByIdAsync(int projectId)
    {
        if (projectId <= 0)
        {
            return null;
        }

        Project? project = await _projectRepository.GetProjectByIdAsync(projectId);
        if (project == null)
        {
            return null;
        }

        List<User> projectManagers = await _projectRepository.GetProjectManagersAsync();

        List<UserViewModel> managers = projectManagers.Select(pm => new UserViewModel
        {
            UserId = pm.UserId,
            Name = pm.Name ?? string.Empty,
            Email = pm.Email ?? string.Empty
        }).ToList();

        return new ProjectViewModel
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.CreatedOn,
            EndDate = project.EndDate,
            AssignedToPM = project.AssignedToPM,
            PMName = project.AssignedPM?.Name ?? string.Empty,
            ProjectManagers = managers,
            Status = project.Status
        };
    }

    public async Task<int> UpdateProjectAsync(ProjectViewModel model, int userId)
    {
        if (model == null || model.ProjectId <= 0 || userId <= 0)
        {
            return 0;
        }
        TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        Project project = new()
        {
            ProjectId = model.ProjectId ?? 0,
            ProjectName = model.ProjectName?.Trim(),
            Description = model.Description,
            AssignedToPM = model.AssignedToPM,
            CreatedOn = TimeZoneInfo.ConvertTimeToUtc(model.StartDate ?? DateTime.UtcNow, localZone),
            EndDate = TimeZoneInfo.ConvertTimeToUtc(model.EndDate ?? DateTime.UtcNow, localZone)
        };

        return await _projectRepository.UpdateProjectAsync(project);
    }

    public async Task<bool> DeleteProjectAsync(int projectId, int userId)
    {
        if (projectId <= 0 || userId <= 0)
        {
            return false;
        }

        return await _projectRepository.DeleteProjectAsync(projectId, userId);
    }

    public async Task<Pagination<MemberViewModel>> GetAssignedMembersAsync(Pagination<MemberViewModel> pagination, int projectId)
    {
        Pagination<User> userPagination = new()
        {
            SearchKeyword = pagination.SearchKeyword,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            SortColumn = pagination.SortColumn,
            SortDirection = pagination.SortDirection
        };

        Pagination<User> members = await _projectRepository.GetPaginatedMembersAsync(userPagination);
        List<ProjectUser> assignedMembers = await _projectRepository.GetAssignedMembers(projectId);

        List<int?> assignedUserIds = assignedMembers.Select(am => am.UserId).ToList();

        List<MemberViewModel> memberViewModels = [];
        foreach (User member in members.Items)
        {
            memberViewModels.Add(new MemberViewModel()
            {
                UserId = member.UserId,
                Name = member.Name,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                IsAssigned = assignedUserIds.Contains(member.UserId)
            });
        }

        return new Pagination<MemberViewModel>
        {
            Items = memberViewModels,
            TotalPages = members.TotalPages,
            TotalRecords = members.TotalRecords
        };
    }

    public async Task<bool> AssignMembersAsync(int projectId, List<int>? userIds)
    {
        return await _projectRepository.AssignMembersAsync(projectId, userIds);
    }

    public async Task<List<ProjectDropDown>> GetProjectNamesAsync(int userId, bool isAdmin)
    {
        List<Project> projects = await _projectRepository.GetProjectNamesAsync(userId, isAdmin);

        List<ProjectDropDown> viewProjects = [];
        foreach (Project project in projects)
        {
            viewProjects.Add(new ProjectDropDown
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName
            });
        }

        return viewProjects;
    }
}