namespace ToDoListManagement.Entity.Constants;

public class Constants
{
    #region Error Messages

    public const string ErrorCode404 = "The page you are looking for was not found.";
    public const string ErrorCode500 = "An unexpected server error occurred.";
    public const string ErrorCode403 = "Unauthorized";
    public const string ErrorCode401 = "Unauthenticated";
    public const string ErrorCodeDefault = "An unexpected error occurred.";

    #endregion

    #region Error Status

    public const string ErrorStatus404 = "Oops, the page you're looking for isn't here.";
    public const string ErrorStatus500 = "An unexpected server error occurred.";
    public const string ErrorStatus403 = "You do not have permission to access this resource.";
    public const string ErrorStatus401 = "You are not authenticated to access this resource.";
    public const string ErrorStatusDefault = "An unexpected error occurred.";

    #endregion

    #region ViewModel Error
    
    public const string EmailRequiredError = "Email is required.";
    public const string EmailInvalidError = "Invalid Email";
    public const string PasswordRequiredError = "Password is required.";
    public const string ValidPasswordMessage = "Password must contain minimum 8 characters and at least one uppercase letter, one lowercase letter, one number, and one special character (.@$!%*?&).";
    public const string NameRequiredError = "Name is required.";
    public const string NameMaxLengthError = "Name cannot be longer than 100 characters.";
    public const string EmailAlreadyExistsError = "Email already registered.";
    public const string ConfirmPasswordRequiredError = "Confirm Password is required.";
    public const string PasswordMismatchError = "Password and Confirm Password do not match.";
    public const string AssignedToPMRequiredError = "Assigned PM is required.";
    public const string TitleRequiredError = "Title is required.";
    public const string TitleMaxLengthError = "Title cannot be longer than 100 characters.";
    public const string AssignedToRequiredError = "Assigned To is required.";
    public const string StartDateRequiredError = "Start Date is required.";
    public const string DueDateRequiredError = "Due Date is required.";
    public const string PriorityRequiredError = "Priority is required.";
    public const string RoleRequiredError = "Role is required.";
    public const string PhoneNumberRequiredError = "Phone Number is required.";
    public const string StatusRequiredError = "Status is required.";
    public const string RoleNameRequiredError = "Role Name is required.";
    public const string RoleNameAlreadyExistsError = "Role Name already taken.";

    #endregion

    #region Login

    public const string LoginSuccessMessage = "Login successful!";
    public const string LoginErrorMessage = "Invalid Credentials! Please try again.";

    #endregion

    #region Registration

    public const string RegistrationSuccessMessage = "Registration successful! Please log in.";
    public const string RegistrationErrorMessage = "Registration failed! Please try again.";
    
    #endregion

    #region Forgot Password

    public const string EmailNotRegisteredMessage = "Email not registered!";
    public const string InvalidEmailMessage = "Invalid email address.";
    public const string InvalidResetPasswordLinkMessage = "Invalid reset password link.";

    #endregion

    #region Reset Password

    public const string ResetPasswordEmailSubject = "Reset Password";
    public const string ResetPasswordEmailBody = "Click the link below to reset your password: <a href='{ResetLink}' target='_blank'>Reset Password</a>";
    public const string ResetPasswordSuccessMessage = "Password reset successfully! Please log in.";
    public const string ResetPasswordErrorMessage = "Failed to reset password. Please try again.";

    #endregion

    #region Project

    public const string ProjectAddedMessage = "Project added successfully.";
    public const string ProjectAddFailedMessage = "Failed to add the project.";
    public const string ProjectAlreadyExistsMessage = "Project name already exists.";
    public const string ProjectNotFoundMessage = "Project not found.";
    public const string ProjectUpdatedMessage = "Project updated successfully.";
    public const string ProjectUpdateFailedMessage = "Failed to update the project.";
    public const string ProjectDeletedMessage = "Project deleted successfully";
    public const string ProjectDeleteFailedMessage = "Failed to delete the project.";
    
    #endregion

    #region Task

    public const string EmployeeAddedMessage = "Employee added successfully.";
    public const string EmployeeAddFailedMessage = "Failed to add the employee.";
    public const string EmployeeUpdatedMessage = "Employee data updated successfully.";
    public const string EmployeeNotFoundError = "Employee not found.";
    public const string EmployeeUpdateFailedMessage = "Failed to update the employee data.";
    public const string EmployeeDeletedMessage = "Employee deleted successfully";
    public const string EmployeeDeleteFailedMessage = "Failed to delete the employee.";

    #endregion

    #region Task

    public const string TaskAddedMessage = "Task added successfully.";
    public const string TaskAddFailedMessage = "Failed to add task.";
    public const string TaskTitleAlreadyExistsError = "Task title already registered.";
    public const string TaskStausChangeMessage = "Task status updated successfully.";
    public const string TaskStausChangeFailedMessage = "Failed to update task status.";
    public const string TaskUpdatedMessage = "Task updated successfully.";
    public const string TaskUpdateFailedMessage = "Failed to update the task.";
    public const string TaskDeletedMessage = "Task deleted successfully.";
    public const string TaskDeleteFailedMessage = "Failed to delete task."; 
    public const string TaskAttachmentDeletedMessage = "Task deleted successfully.";
    public const string TaskAttachmentDeleteFailedMessage = "Failed to delete task.";

    #endregion

    #region Role

    public const string RoleNotFoundMessage = "Role not found.";
    public const string RoleUpdatedMessage = "Role updated successfully.";
    public const string RoleUpdateFailedMessage = "Failed to update the role.";
    public const string RoleDeletedMessage = "Role deleted successfully";
    public const string RoleDeleteFailedMessage = "Failed to delete the role.";

    #endregion

    #region Permission

    public static readonly string[] Tabs = ["Projects", "Employees", "Task Board", "Role And Permissions"];
    public const string PermissionUpdatedMessage = "Permissions updated successfully.";
    public const string PermissionUpdateFailedMessage = "Failed to update the permissions.";

    #endregion

}
