using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface ITaskService
{
    Task<ProjectTasksViewModel> GetProjectNamesAsync(UserViewModel user);
    Task<List<TaskViewModel>?> GetTasksByProjectIdAsync(int projectId);
    Task<bool> UpdateTaskStatusAsync(int taskId, string newStatus);
    Task<bool> UpdateTaskAsync(TaskViewModel model, int userId);
    Task<TaskViewModel> GetTeamMembersAsync(int projectId);
    Task<int> AddTaskAsync(TaskViewModel model, UserViewModel user);
    Task<TaskViewModel?> GetTaskByIdAsync(int taskId);
    Task<(FileStream? Stream, string? FileName)> DownloadFileAsync(int taskAttachmentId);
    Task<int?> DeleteTaskAsync(int taskId);
}
