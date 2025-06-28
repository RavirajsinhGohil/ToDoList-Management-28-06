using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface IEmployeeService
{
    Task<Pagination<EmployeeViewModel>> GetPaginatedEmployeesAsync(Pagination<EmployeeViewModel> pagination, UserViewModel user);
    Task<int> AddEmployeeAsync(EmployeeViewModel employee);
    Task<EditEmployeeViewModel> GetEmployeeByIdAsync(int employeeId);
    Task<int> UpdateEmployeeAsync(EditEmployeeViewModel employee);
    Task<int> DeleteEmployeeAsync(int employeeId);
}
