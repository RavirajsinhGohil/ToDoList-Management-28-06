using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ToDoListDbContext _context;
    public EmployeeRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<Pagination<User>> GetPaginatedEmployeesAsync(Pagination<User> pagination, int userId)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        IQueryable<User> query = _context.Users
            .Where(u => u.UserId != userId && !u.IsDeleted).AsQueryable();

        if (user != null && user.RoleId == 2)
        {
            query = query.Where(u => u.RoleId != 1);
        }

        if (!string.IsNullOrEmpty(pagination.SearchKeyword))
        {
            query = query.Where(p => !string.IsNullOrEmpty(p.Name) && 
                                      !string.IsNullOrEmpty(pagination.SearchKeyword) && 
                                      p.Name.ToLower().Contains(pagination.SearchKeyword.Trim().ToLower()));
        }

        int totalRecords = await query.CountAsync();

        query = pagination.SortColumn switch
        {
            "email" => (pagination.SortDirection?.ToLower() ?? "asc") == "asc"
                                ? query.OrderBy(p => p.Email)
                                : query.OrderByDescending(p => p.Email),

            "role" => (pagination.SortDirection?.ToLower() ?? "asc") == "asc"
                                ? query.OrderBy(p => p.Role)
                                : query.OrderByDescending(p => p.Role),

            _ => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.Name)
                                : query.OrderByDescending(p => p.Name),
        };

        List<User> pagedData = await query
            .Skip((pagination.CurrentPage - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        int totalPages = (int)Math.Ceiling((double)totalRecords / pagination.PageSize);

        return new Pagination<User>
        {
            Items = pagedData,
            CurrentPage = pagination.CurrentPage,
            TotalPages = totalPages,
            TotalRecords = totalRecords
        };
    }

    public async Task<int> AddEmployeeAsync(User user)
    {
        if (user == null)
        {
            return 0;
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return 1;
    }

    public async Task<User> GetEmployeeByIdAsync(int employeeId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == employeeId && !u.IsDeleted) 
            ?? throw new InvalidOperationException("Employee not found.");
    }

    public async Task<int> UpdateEmployeeAsync(User user)
    {
        if (user == null || user.UserId <= 0)
        {
            return 0;
        }

        User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId && !u.IsDeleted);
        if (existingUser == null)
        {
            return -1;
        }

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.Role = user.Role;
        existingUser.IsActive = user.IsActive;

        _context.Users.Update(existingUser);
        return await _context.SaveChangesAsync();
    }
    
    public async Task<int> DeleteEmployeeAsync(int employeeId)
    {
        if (employeeId <= 0)
        {
            return 0;
        }

        User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == employeeId && !u.IsDeleted);
        if (existingUser == null)
        {
            return -1;
        }

        existingUser.IsDeleted = true;
        _context.Users.Update(existingUser);
        return await _context.SaveChangesAsync();
    }
}