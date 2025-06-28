using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface ITaskRepository
{
    Task<List<ToDoList>> GetTasksByProjectIdAsync(int projectId);
    Task<ToDoList?> GetTaskByIdAsync(int taskId);
    Task<bool> UpdateTaskAsync(ToDoList task);
    Task<List<ProjectUser>> GetTeamMembersAsync(int projectId);
    Task<int> AddTaskAsync(ToDoList task);
    Task<bool> AddAttachmentAsync(TaskAttachment taskAttachment);
    Task<List<TaskAttachment>> GetTaskAttachmentsByTaskIdAsync(int taskId);
    Task<TaskAttachment?> GetTaskAttachmentAsync(int taskAttachmentId);
    Task<bool> DeleteTaskAttachmentAsync(TaskAttachment taskAttachment);
}
