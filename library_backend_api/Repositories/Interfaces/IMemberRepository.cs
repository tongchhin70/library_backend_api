using library_backend_api.Entities;

namespace library_backend_api.Repositories.Interfaces;

// Member repository contract for database access.
public interface IMemberRepository
{
    Task<List<Member>> GetAllAsync();
    Task<Member?> GetByIdAsync(int id);
    Task<Member> AddAsync(Member member);
    Task UpdateAsync(Member member);
    Task DeleteAsync(Member member);
    Task<bool> ExistsByEmailAsync(string email, int? excludedId = null);
}
