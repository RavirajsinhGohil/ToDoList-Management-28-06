using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface IEmployeeRepository
{
    Task<Pagination<User>> GetPaginatedEmployeesAsync(Pagination<User> pagination, int userId);
    Task<int> AddEmployeeAsync(User user);
    Task<User> GetEmployeeByIdAsync(int employeeId);
    Task<int> UpdateEmployeeAsync(User user);
    Task<int> DeleteEmployeeAsync(int employeeId);
}
