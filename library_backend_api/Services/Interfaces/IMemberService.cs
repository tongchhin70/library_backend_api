using library_backend_api.DTOs.Members;

namespace library_backend_api.Services.Interfaces;

// Member service contract for CRUD operations on library members.
public interface IMemberService
{
    Task<IEnumerable<MemberResponseDto>> GetAllMembersAsync();
    Task<MemberResponseDto?> GetMemberByIdAsync(int id);
    Task<MemberResponseDto> CreateMemberAsync(MemberCreateDto dto);
    Task<bool> UpdateMemberAsync(int id, MemberUpdateDto dto);
    Task<bool> DeleteMemberAsync(int id);
}
