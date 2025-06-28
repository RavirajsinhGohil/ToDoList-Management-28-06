using System.ComponentModel.DataAnnotations;

namespace ToDoListManagement.Entity.ViewModel;

public class ProfileViewModel
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = Constants.Constants.NameRequiredError)]
    [StringLength(100, ErrorMessage = Constants.Constants.NameMaxLengthError)]
    public string? Name { get; set; }

    [Required(ErrorMessage = Constants.Constants.EmailRequiredError)]
    [EmailAddress(ErrorMessage = Constants.Constants.EmailInvalidError)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public int Role { get; set; }

    [Required(ErrorMessage = "Phone Number is required.")]
    [RegularExpression(@"^\(?([0-9]{10})\)$", ErrorMessage = "Invalid Phone Number")]
    public string? PhoneNumber { get; set; }
}
