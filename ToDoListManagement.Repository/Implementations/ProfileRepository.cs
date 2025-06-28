using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class ProfileRepository : IProfileRepository
{
    private readonly ToDoListDbContext _context;

    public ProfileRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        return user ?? null;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId && !u.IsDeleted);

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.Role = user.Role;

        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();

        return true;
    }
}
