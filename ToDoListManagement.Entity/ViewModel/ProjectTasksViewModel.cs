namespace ToDoListManagement.Entity.ViewModel;

public class ProjectTasksViewModel
{
    public int ProjectId { get; set; }
    public ProjectViewModel? ProjectView { get; set; }
    public List<TaskViewModel>? Tasks { get; set; }
    public List<ProjectDropDown>? Projects { get; set; }
}

public class ProjectDropDown{
    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
}
