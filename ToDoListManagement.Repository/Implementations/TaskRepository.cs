using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class TaskRepository : ITaskRepository
{
    private readonly ToDoListDbContext _context;

    public TaskRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<List<ToDoList>> GetTasksByProjectIdAsync(int projectId)
    {
        return await _context.ToDoLists.Where(t => t.ProjectId == projectId && !t.IsDeleted).ToListAsync();
    }

    public async Task<ToDoList?> GetTaskByIdAsync(int taskId)
    {
        return await _context.ToDoLists.Include(t => t.Project).FirstOrDefaultAsync(t => t.TaskId == taskId);
    }

    public async Task<bool> UpdateTaskAsync(ToDoList task)
    {
        _context.ToDoLists.Update(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProjectUser>> GetTeamMembersAsync(int projectId)
    {
        return await _context.ProjectUsers.Where(pu => pu.ProjectId == projectId && pu.User != null && pu.User.RoleId != 2).Include(pu => pu.User).ToListAsync();
    }

    public async Task<int> AddTaskAsync(ToDoList task)
    {
        ToDoList? existingTask = await _context.ToDoLists.FirstOrDefaultAsync(p => p.Title != null && task.Title != null && p.Title.ToLower() == task.Title.Trim().ToLower());
        if (existingTask != null)
        {
            return -1;
        }

        await _context.ToDoLists.AddAsync(task);
        await _context.SaveChangesAsync();
        return task.TaskId;
    }

    public async Task<bool> AddAttachmentAsync(TaskAttachment taskAttachment)
    {
        await _context.TaskAttachments.AddAsync(taskAttachment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskAttachment>> GetTaskAttachmentsByTaskIdAsync(int taskId)
    {
        return await _context.TaskAttachments.Where(ta => ta.TaskId == taskId && !ta.IsDeleted).ToListAsync();
    }

    public async Task<TaskAttachment?> GetTaskAttachmentAsync(int taskAttachmentId)
    {
        return await _context.TaskAttachments
            .FirstOrDefaultAsync(a => a.AttachmentId == taskAttachmentId && !a.IsDeleted);
    }

    public async Task<bool> DeleteTaskAttachmentAsync(TaskAttachment taskAttachment)
    {
        TaskAttachment? attachment = await GetTaskAttachmentAsync(taskAttachment.AttachmentId);
        if (attachment == null) return false;

        attachment = taskAttachment;
        _context.TaskAttachments.Update(attachment);
        await _context.SaveChangesAsync();
        return true;
    }
}
