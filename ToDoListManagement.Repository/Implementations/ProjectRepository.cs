using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class ProjectRepository : IProjectRepository
{
    private readonly ToDoListDbContext _context;
    public ProjectRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddProject(Project project, int pMId)
    {
        Project? existingProject = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectName != null && project.ProjectName != null && p.ProjectName.ToLower() == project.ProjectName.ToLower());
        if (existingProject != null)
        {
            return -1;
        }

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        ProjectUser projectUser = new()
        {
            ProjectId = project.ProjectId,
            UserId = pMId,
        };
        await _context.ProjectUsers.AddAsync(projectUser);
        await _context.SaveChangesAsync();

        return project.ProjectId;
    }

    public async Task<Project?> GetProjectByIdAsync(int projectId)
    {
        if (projectId <= 0)
        {
            return null;
        }

        return await _context.Projects
            .Include(p => p.ProjectUsers)
            .Include(p => p.AssignedPM)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && !p.IsDeleted);
    }

    public async Task<List<User>> GetProjectManagersAsync()
    {
        return await _context.Users
            .Where(u => u.RoleId == 2 && !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> UpdateProjectAsync(Project project)
    {
        if (project == null || project.ProjectId <= 0)
        {
            return 0;
        }

        Project? existingProject = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == project.ProjectId && !p.IsDeleted);
        if (existingProject == null)
        {
            return -1;
        }

        existingProject.ProjectName = project.ProjectName;
        existingProject.Description = project.Description;
        existingProject.AssignedToPM = project.AssignedToPM;
        existingProject.CreatedOn = project.CreatedOn;
        existingProject.EndDate = project.EndDate;

        _context.Projects.Update(existingProject);
        await _context.SaveChangesAsync();
        return 1;
    }

    public async Task<Pagination<User>> GetPaginatedMembersAsync(Pagination<User> pagination)
    {
        IQueryable<User>? query = _context.Users.Where(u => u.RoleId != 1 && u.RoleId != 2 && !u.IsDeleted);

        int totalRecords = await query.CountAsync();

        if (!string.IsNullOrEmpty(pagination.SearchKeyword))
        {
            query = query.Where(u => u.Name != null && u.Name.ToLower().Contains(pagination.SearchKeyword.Trim().ToLower()) || u.Email != null && u.Email.Contains(pagination.SearchKeyword.Trim().ToLower()) || u.PhoneNumber != null && u.PhoneNumber.Contains(pagination.SearchKeyword.Trim().ToLower()));
        }

        query = pagination.SortColumn switch
        {
            "name" => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.Name)
                                : query.OrderByDescending(p => p.Name),
            "email" => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.Email)
                                : query.OrderByDescending(p => p.Email),
            "phoneNumber" => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.PhoneNumber)
                                : query.OrderByDescending(p => p.PhoneNumber),
            _ => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.UserId)
                                : query.OrderByDescending(p => p.UserId),
        };

        int totalPages = (int)Math.Ceiling((double)totalRecords / pagination.PageSize);

        List<User> pagedData = await query
            .Skip((pagination.CurrentPage - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new Pagination<User>
        {
            Items = pagedData,
            CurrentPage = pagination.CurrentPage,
            TotalPages = totalPages,
            TotalRecords = totalRecords
        };
    }


    public async Task<List<ProjectUser>> GetAssignedMembers(int projectId)
    {
        return await _context.ProjectUsers.Include(pu => pu.User).Where(pu => pu.ProjectId == projectId && pu.User != null && pu.User.RoleId != 1 && pu.User.RoleId != 2).ToListAsync();
    }

    public async Task<bool> DeleteProjectAsync(int projectId, int userId)
    {
        Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId && !p.IsDeleted);

        if (project == null)
        {
            return false;
        }

        project.IsDeleted = true;
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AssignMembersAsync(int projectId, List<int>? userIds)
    {
        IQueryable<ProjectUser>? existingUsers = _context.ProjectUsers.Where(pu => pu.ProjectId == projectId && pu.User != null && pu.User.RoleId != 2);
        _context.ProjectUsers.RemoveRange(existingUsers);

        IEnumerable<ProjectUser>? newProjectUsers = (userIds ?? new List<int>()).Select(id => new ProjectUser
        {
            ProjectId = projectId,
            UserId = id
        });

        await _context.ProjectUsers.AddRangeAsync(newProjectUsers);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Project>> GetProjectNamesAsync(int userId, bool isAdmin)
    {
        IQueryable<Project> query = _context.Projects
            .Where(p => !p.IsDeleted)
            .Include(pu => pu.ProjectUsers)
            .ThenInclude(u => u.User)
            .Include(p => p.AssignedPM);

        if (!isAdmin)
        {
            query = query.Where(pu => pu.ProjectUsers != null && pu.ProjectUsers.Any(u => u.UserId == userId && u.User != null && !u.User.IsDeleted));
        }

        return await query.OrderBy(p => p.ProjectName).ToListAsync();
    }

}
