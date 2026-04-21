using library_backend_api.Data;
using library_backend_api.Entities;
using library_backend_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace library_backend_api.Repositories.Implementations;

// Member repository contains only database access for member records.
public class MemberRepository(AppDbContext context) : IMemberRepository
{
    public Task<List<Member>> GetAllAsync() =>
        context.Members.AsNoTracking().OrderBy(member => member.Id).ToListAsync();

    public Task<Member?> GetByIdAsync(int id) =>
        context.Members.AsNoTracking().FirstOrDefaultAsync(member => member.Id == id);

    public async Task<Member> AddAsync(Member member)
    {
        context.Members.Add(member);
        await context.SaveChangesAsync();
        return member;
    }

    public async Task UpdateAsync(Member member)
    {
        context.Members.Update(member);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Member member)
    {
        context.Members.Remove(member);
        await context.SaveChangesAsync();
    }

    public Task<bool> ExistsByEmailAsync(string email, int? excludedId = null) =>
        context.Members.AnyAsync(member => member.Email == email && (!excludedId.HasValue || member.Id != excludedId.Value));
}
