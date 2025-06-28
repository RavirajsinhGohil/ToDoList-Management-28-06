using System.ComponentModel.DataAnnotations;

namespace ToDoListManagement.Entity.ViewModel;

public class ProjectViewModel
{
    public int? ProjectId { get; set; }

    [Required(ErrorMessage = Constants.Constants.NameRequiredError)]
    [MaxLength(50)]
    public string? ProjectName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = Constants.Constants.StartDateRequiredError)]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = Constants.Constants.DueDateRequiredError)]
    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    [Required(ErrorMessage = Constants.Constants.AssignedToPMRequiredError)]
    public int? AssignedToPM { get; set; }

    public string? PMName { get; set; }

    public List<UserViewModel> ProjectManagers { get; set; } = [];

    public List<UserViewModel> Users { get; set; } = [];
}