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
        var members = await memberService.GetAllMembersAsync();
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMemberById(int id)
    {
        var member = await memberService.GetMemberByIdAsync(id);
        if (member == null) return NotFound(new { error = $"Member with ID {id} not found." });

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
        var updated = await memberService.UpdateMemberAsync(id, dto);
        if (!updated) return NotFound(new { error = $"Member with ID {id} not found." });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(int id)
    {
        var deleted = await memberService.DeleteMemberAsync(id);
        if (!deleted) return NotFound(new { error = $"Member with ID {id} not found." });

        return NoContent();
    }
}
