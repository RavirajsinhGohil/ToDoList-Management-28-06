using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Pagination<EmployeeViewModel>> GetPaginatedEmployeesAsync(Pagination<EmployeeViewModel> pagination, UserViewModel user)
    {
        Pagination<User> employeePagination = new(){
            SearchKeyword = pagination.SearchKeyword,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            SortColumn = pagination.SortColumn,
            SortDirection = pagination.SortDirection
        };

        Pagination<User> employees = await _employeeRepository.GetPaginatedEmployeesAsync(employeePagination, user.UserId);

        List<EmployeeViewModel> employeeViewModels = [];
        foreach(User employee in employees.Items)
        {
            employeeViewModels.Add(new EmployeeViewModel
            {
                EmployeeId = employee.UserId,
                Name = employee.Name ?? string.Empty,
                Email = employee.Email ?? string.Empty,
                PhoneNumber = employee.PhoneNumber ?? string.Empty,
                Role = employee.RoleId,
                Status = employee.IsActive ? "Active" : "Inactive",
            });
        }

        return new Pagination<EmployeeViewModel>{
            Items = employeeViewModels,
            TotalPages = employees.TotalPages,
            TotalRecords = employees.TotalRecords
        };
    }

    public async Task<int> AddEmployeeAsync(EmployeeViewModel employee)
    {
        User user = new()
        {
            Name = employee.Name?.Trim(),
            Email = employee.Email?.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(employee.Password?.Trim()), 
            PhoneNumber = employee.PhoneNumber,
            RoleId = employee.Role
        };
        return await _employeeRepository.AddEmployeeAsync(user);
    }

    public async Task<EditEmployeeViewModel> GetEmployeeByIdAsync(int employeeId)
    {
        User user = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        if (user == null)
        {
            return new EditEmployeeViewModel();
        }
        return new EditEmployeeViewModel
        {
            EmployeeId = user.UserId,
            Name = user.Name ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Role = user.RoleId,
            Status = user.IsActive ? "Active" : "Inactive",
        };
    }

    public async Task<int> UpdateEmployeeAsync(EditEmployeeViewModel employee)
    {
        User user = new()
        {
            UserId = employee.EmployeeId,
            Name = employee.Name?.Trim(),
            Email = employee.Email?.Trim().ToLower(),
            PhoneNumber = employee.PhoneNumber,
            RoleId = employee.Role,
            IsActive = employee.Status == "Active" ? true : false
        };
        return await _employeeRepository.UpdateEmployeeAsync(user);
    }

    public async Task<int> DeleteEmployeeAsync(int employeeId)
    {
        return await _employeeRepository.DeleteEmployeeAsync(employeeId);
    }
}