using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync(int? userId, string? role)
    {
        DashboardViewModel model = new();

        List<User> projectManagers = await _dashboardRepository.GetProjectManagersAsync(userId);

        model.ProjectManagers = projectManagers.Select(pm => new UserViewModel
        {
            UserId = pm.UserId,
            Name = pm.Name ?? string.Empty,
            Email = pm.Email ?? string.Empty
        }).ToList();

        return model;
    }

    public async Task<Pagination<ProjectViewModel>> GetPaginatedProjectsAsync(Pagination<ProjectViewModel> pagination, UserViewModel user)
    {
        Pagination<Project> projectPagination = new(){
            SearchKeyword = pagination.SearchKeyword,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            SortColumn = pagination.SortColumn,
            SortDirection = pagination.SortDirection
        };

        bool isAdmin = user.Role == "Admin";

        Pagination<Project> projects = await _dashboardRepository.GetPaginatedProjectsAsync(projectPagination, user.UserId, isAdmin);
        List<ProjectViewModel> projectViewModels = new();
        foreach (Project project in projects.Items)
        {
            List<UserViewModel> users = [];
            if (project.ProjectUsers != null )
            {
                foreach (ProjectUser? projectUser in project.ProjectUsers)
                {
                    if(projectUser.User?.RoleId != 2)
                    {
                        users.Add(new UserViewModel
                        {
                            UserId = projectUser.User?.UserId ?? 0,
                            Name = projectUser.User?.Name ?? string.Empty,
                            Email = projectUser.User?.Email ?? string.Empty
                        });
                    }
                }
            }
            projectViewModels.Add(new ProjectViewModel
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName ?? string.Empty,
                Description = project.Description ?? string.Empty,
                StartDate = project.CreatedOn.HasValue ? project.CreatedOn.Value.ToLocalTime() : null,
                PMName = project.AssignedPM?.Name,
                EndDate = project.EndDate,
                Status = project.Status ?? string.Empty,
                Users = users
            });
        }

        return new Pagination<ProjectViewModel>{
            Items = projectViewModels,
            TotalPages = projects.TotalPages,
            TotalRecords = projects.TotalRecords
        };
    }
}
