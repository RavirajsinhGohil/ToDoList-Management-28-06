using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Helper;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

public class ProjectController : BaseController
{
    private readonly IProjectService _projectService;
    public ProjectController(IAuthService authService, IProjectService projectService) : base(authService)
    {
        _projectService = projectService;
    }

    [CustomAuthorize("Projects", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> AddProject(DashboardViewModel model)
    {
        if (model.Project == null)
        {
            TempData["ErrorMessage"] = Constants.ProjectAddFailedMessage;
            return RedirectToAction("Index", "Dashboard");
        }

        if (SessionUser == null)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        int projectId = await _projectService.AddProject(model.Project, SessionUser.UserId);
        if (projectId > 0)
        {
            TempData["SuccessMessage"] = Constants.ProjectAddedMessage;
        }
        else if (projectId == -1)
        {
            TempData["ErrorMessage"] = Constants.ProjectAlreadyExistsMessage;
        }
        else
        {
            TempData["ErrorMessage"] = Constants.ProjectAddFailedMessage;
        }
        return RedirectToAction("Index", "Dashboard");
    }

    [CustomAuthorize("Projects", "CanAddEdit")]
    [HttpGet]
    public async Task<IActionResult> GetProjectById(int projectId)
    {
        ProjectViewModel? model = await _projectService.GetProjectByIdAsync(projectId);
        if (model == null)
        {
            return Json(new { success = false, message = Constants.ProjectNotFoundMessage });
        }
    
        return PartialView("_UpdateProjectModal", model);
    }

    [CustomAuthorize("Projects", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> UpdateProject(ProjectViewModel model)
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        int? projectId = await _projectService.UpdateProjectAsync(model, SessionUser.UserId);
        if (projectId.HasValue && projectId.Value > 0)
        {
            TempData["SuccessMessage"] = Constants.ProjectUpdatedMessage;
        }
        else if (projectId == -1)
        {
            TempData["ErrorMessage"] = Constants.ProjectAlreadyExistsMessage;
        }
        else
        {
            TempData["ErrorMessage"] = Constants.ProjectUpdateFailedMessage;
        }
        return RedirectToAction("Index", "Dashboard");
    }

    [CustomAuthorize("Projects", "CanAddEdit")]
    [HttpGet]
    public async Task<IActionResult> GetAssignedMembers(int projectId, int draw, int start, int length, string searchValue, string sortColumn, string sortDirection)
    {
        int pageNumber = start / length + 1;
        int pageSize = length;

        Pagination<MemberViewModel>? pagination = new()
        {
            SearchKeyword = searchValue,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn,
            SortDirection = sortDirection
        };
        
        Pagination<MemberViewModel> data = await _projectService.GetAssignedMembersAsync(pagination, projectId);

        return Json(new
        {
            draw,
            recordsTotal = data.TotalRecords,
            recordsFiltered = data.TotalRecords,
            data = data.Items
        });
    }

    [CustomAuthorize("Projects", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> AssignMembers([FromBody] AssignMembersViewModel request)
    {
        bool isAssigned =  await _projectService.AssignMembersAsync(request.ProjectId, request.UserIds);
        if(isAssigned)
        {
            return Json(new { success = true, message = "Assigned Successfully!" });
        }
        else
        {
            return Json(new { success = false, message = "Failed to assigned members!" });
        }
    }

    [CustomAuthorize("Projects", "CanDelete")]
    [HttpGet]
    public async Task<IActionResult> DeleteProject(int projectId)
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        bool isDeleted = await _projectService.DeleteProjectAsync(projectId, SessionUser.UserId);
        if (isDeleted)
        {
            TempData["SuccessMessage"] = Constants.ProjectDeletedMessage;
        }
        else
        {
            TempData["ErrorMessage"] = Constants.ProjectDeleteFailedMessage;
        }
        
        return RedirectToAction("Index", "Dashboard");
    }
}