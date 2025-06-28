using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Helper;
using ToDoListManagement.Service.Interfaces;
using ToDoListManagement.Web.Hub;

namespace ToDoListManagement.Web.Controllers;

[Authorize]
public class TasksController : BaseController
{
    private readonly ITaskService _taskService;
    private readonly IHubContext<ChatHub> _hubContext;

    public TasksController(IAuthService authService, ITaskService taskService, IHubContext<ChatHub> hubContext)
        : base(authService)
    {
        _taskService = taskService;
        _hubContext = hubContext;
    }

    [CustomAuthorize("Task Board", "CanView")]
    [HttpGet]
    public async Task<IActionResult> Index(int? projectId)
    {
        if (SessionUser != null)
        {
            ProjectTasksViewModel model = await _taskService.GetProjectNamesAsync(SessionUser);
            model.ProjectId = projectId ?? 0;
            if (model.ProjectId != 0)
            {
                model.Tasks = await _taskService.GetTasksByProjectIdAsync(model.ProjectId) ?? new List<TaskViewModel>();
            }
            return View(model);
        }
        return RedirectToAction("Login", "Auth");
    }

    [CustomAuthorize("Task Board", "CanView")]
    [HttpGet]
    public async Task<IActionResult> GetTasksByProjectId(int projectId)
    {
        List<TaskViewModel>? tasks = await _taskService.GetTasksByProjectIdAsync(projectId);
        return PartialView("_TasksList", tasks ?? new List<TaskViewModel>());
    }

    [CustomAuthorize("Task Board", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int taskId, string newStatus)
    {
        bool result = await _taskService.UpdateTaskStatusAsync(taskId, newStatus);
        if (result)
        {
            return Json(new { success = true, message = Constants.TaskStausChangeMessage});
        }
        return Json(new { success = false, message = Constants.TaskStausChangeFailedMessage});
    }

    [CustomAuthorize("Task Board", "CanView")]
    [HttpGet]
    public async Task<IActionResult> GetTeamMembers(int projectId)
    {
        TaskViewModel task = await _taskService.GetTeamMembersAsync(projectId);
        return PartialView("_AddTaskModal", task);
    }

    [CustomAuthorize("Task Board", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> AddTask(TaskViewModel model)
    {
        int isAdded = await _taskService.AddTaskAsync(model, SessionUser);
        if (isAdded == 1)
        {
            TempData["SuccessMessage"] = Constants.TaskAddedMessage;
            await _hubContext.Clients.All.SendAsync("NewTaskAdded");
        }
        else if(isAdded == -1)
        {
            TempData["ErrorMessage"] = Constants.TaskTitleAlreadyExistsError;
        }
        else
        {
            TempData["ErrorMessage"] = Constants.TaskAddFailedMessage;
        }
        return RedirectToAction("Index", new { projectId = model.ProjectId });
    }

    // [CustomAuthorize("Task Board", "CanAddEdit")]
    [HttpGet]
    public async Task<IActionResult> GetTaskById(int taskId)
    {
        TaskViewModel? task = await _taskService.GetTaskByIdAsync(taskId);
        return PartialView("_UpdateTaskModal", task);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadFile(int taskAttachmentId)
    {
        (FileStream? fileStream, string? fileName) = await _taskService.DownloadFileAsync(taskAttachmentId);

        if (fileStream != null && !string.IsNullOrEmpty(fileName))
        {
            return File(fileStream, "application/octet-stream", fileName);
        }

        return NotFound();
    }

    [CustomAuthorize("Task Board", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> UpdateTask(TaskViewModel model)
    {
        bool isUpdated = await _taskService.UpdateTaskAsync(model, SessionUser.UserId);
        if (isUpdated)
        {
            TempData["SuccessMessage"] = Constants.TaskUpdatedMessage;
            await _hubContext.Clients.All.SendAsync("TaskUpdated");
        }
        else
        {
            TempData["ErrorMessage"] = Constants.TaskUpdateFailedMessage;
        }
        return RedirectToAction("Index", new { projectId = model.ProjectId });
    }

    [CustomAuthorize("Task Board", "CanDelete")]
    [HttpGet]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        int? projectId = await _taskService.DeleteTaskAsync(taskId);
        if (projectId > 0)
        {
            TempData["SuccessMessage"] = Constants.TaskDeletedMessage;
            await _hubContext.Clients.All.SendAsync("TaskDeleted");
        }
        else
        {
            TempData["ErrorMessage"] = Constants.TaskDeleteFailedMessage;
        }
        return RedirectToAction("Index", new { projectId });
    }
}