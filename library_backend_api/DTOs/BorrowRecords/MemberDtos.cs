namespace library_backend_api.DTOs.Members;
using System.ComponentModel.DataAnnotations;


public class MemberResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }
}

public class MemberCreateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class MemberUpdateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
