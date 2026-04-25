using library_backend_api.DTOs.Members;
using library_backend_api.Entities;
using library_backend_api.Repositories.Interfaces;
using library_backend_api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace library_backend_api.Services.Implementations;

public class MemberService(IMemberRepository repository, IBorrowRecordRepository borrowRecordRepository, IMemoryCache cache) : IMemberService
{
    private const string AllMembersCacheKey = "AllMembersCache";

    public async Task<IEnumerable<MemberResponseDto>> GetAllMembersAsync()
    {
        if (cache.TryGetValue(AllMembersCacheKey, out IEnumerable<MemberResponseDto>? cachedMembers))
        {
            return cachedMembers!;
        }

        var members = await repository.GetAllAsync();
        var dtos = members.Select(MapToDto).ToList();

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        cache.Set(AllMembersCacheKey, dtos, cacheOptions);

        return dtos;
    }

    public async Task<MemberResponseDto?> GetMemberByIdAsync(int id)
    {
        var member = await repository.GetByIdAsync(id);
        return member == null ? null : MapToDto(member);
    }

    public async Task<MemberResponseDto> CreateMemberAsync(MemberCreateDto dto)
    {
        if (await repository.ExistsByEmailAsync(dto.Email))
        {
            throw new InvalidOperationException($"A member with email {dto.Email} already exists.");
        }

        var member = new Member
        {
            FullName = dto.FullName,
            Email = dto.Email,
            MembershipDate = DateTime.UtcNow
        };

        var createdMember = await repository.AddAsync(member);

        // INVALIDATE CACHE
        cache.Remove(AllMembersCacheKey);

        return MapToDto(createdMember);
    }

    public async Task<bool> UpdateMemberAsync(int id, MemberUpdateDto dto)
    {
        var existingMember = await repository.GetByIdAsync(id);
        if (existingMember == null) return false;

        if (await repository.ExistsByEmailAsync(dto.Email, id))
        {
            throw new InvalidOperationException($"Another member with email {dto.Email} already exists.");
        }

        existingMember.FullName = dto.FullName;
        existingMember.Email = dto.Email;

        await repository.UpdateAsync(existingMember);

        // INVALIDATE CACHE
        cache.Remove(AllMembersCacheKey);

        return true;
    }

    public async Task<bool> DeleteMemberAsync(int id)
    {
        var member = await repository.GetByIdAsync(id);
        if (member == null) return false;

        // Prevent deletion if the member has any borrow history.
        if (await borrowRecordRepository.HasBorrowHistoryForMemberAsync(id))
        {
            throw new InvalidOperationException("Cannot delete a member with existing borrow history.");
        }

        await repository.DeleteAsync(member);

        // INVALIDATE CACHE
        cache.Remove(AllMembersCacheKey);

        return true;
    }

    private static MemberResponseDto MapToDto(Member member)
    {
        return new MemberResponseDto
        {
            Id = member.Id,
            FullName = member.FullName,
            Email = member.Email,
            MembershipDate = member.MembershipDate
        };
    }
}
