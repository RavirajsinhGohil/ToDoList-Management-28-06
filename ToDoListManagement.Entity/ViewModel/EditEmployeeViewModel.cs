using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListManagement.Entity.ViewModel;

public class EditEmployeeViewModel
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = Constants.Constants.NameRequiredError)]
    [MaxLength(50)]
    [StringLength(100, ErrorMessage = Constants.Constants.NameMaxLengthError)]
    public string? Name { get; set; }

    [Required(ErrorMessage = Constants.Constants.EmailRequiredError)]
    [MaxLength(50)]
    [EmailAddress(ErrorMessage = Constants.Constants.EmailInvalidError)]
    public string? Email { get; set; }

    [Required(ErrorMessage = Constants.Constants.RoleRequiredError)]
    public int Role { get; set; }

    [Required(ErrorMessage = Constants.Constants.PhoneNumberRequiredError)]
    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = Constants.Constants.StatusRequiredError)]
    public string? Status { get; set; }
}
