using library_backend_api.DTOs.Members;
using library_backend_api.Entities;
using library_backend_api.Exceptions;
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

    public async Task<MemberResponseDto> GetMemberByIdAsync(int id)
    {
        var member = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Member with ID {id} not found.");

        return MapToDto(member);
    }

    public async Task<MemberResponseDto> CreateMemberAsync(MemberCreateDto dto)
    {
        if (await repository.ExistsByEmailAsync(dto.Email))
        {
            //duplicate email is not a server crash. It is a conflict with existing data.
            throw new ConflictException($"A member with email {dto.Email} already exists.");
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

    public async Task UpdateMemberAsync(int id, MemberUpdateDto dto)
    {
        var existingMember = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Member with ID {id} not found.");

        if (await repository.ExistsByEmailAsync(dto.Email, id))
        {
            throw new ConflictException($"Another member with email {dto.Email} already exists.");
        }

        existingMember.FullName = dto.FullName;
        existingMember.Email = dto.Email;

        await repository.UpdateAsync(existingMember);

        // INVALIDATE CACHE
        cache.Remove(AllMembersCacheKey);
    }

    public async Task DeleteMemberAsync(int id)
    {
        var member = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Member with ID {id} not found.");

        // Prevent deletion if the member has any borrow history.
        if (await borrowRecordRepository.HasBorrowHistoryForMemberAsync(id))
        {
            // Deleting a member with borrow history is a conflict with data integrity rules.
            throw new ConflictException("Cannot delete a member with existing borrow history.");
        }

        await repository.DeleteAsync(member);

        // INVALIDATE CACHE
        cache.Remove(AllMembersCacheKey);
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
