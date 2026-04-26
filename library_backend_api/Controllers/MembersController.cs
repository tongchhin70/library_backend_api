using library_backend_api.DTOs.Members;
using library_backend_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace library_backend_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MembersController(IMemberService memberService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllMembers()
    {
        // This endpoint retrieves all members. It uses caching in the service layer to improve performance.
        var members = await memberService.GetAllMembersAsync();
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMemberById(int id)
    {
        var member = await memberService.GetMemberByIdAsync(id);
        return Ok(member);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMember([FromBody] MemberCreateDto dto)
    {
        var createdMember = await memberService.CreateMemberAsync(dto);
        return CreatedAtAction(nameof(GetMemberById), new { id = createdMember.Id }, createdMember);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMember(int id, [FromBody] MemberUpdateDto dto)
    {
        await memberService.UpdateMemberAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(int id)
    {
        await memberService.DeleteMemberAsync(id);
        return NoContent();
    }
}
