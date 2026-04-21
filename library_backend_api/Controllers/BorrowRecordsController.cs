using library_backend_api.DTOs.BorrowRecords;
using library_backend_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace library_backend_api.Controllers;

[ApiController]
[Route("api/borrow-records")]
// Borrow endpoints map requests to the borrowing service layer.
public class BorrowRecordsController(IBorrowRecordService borrowRecordService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BorrowRecordResponseDto>>> GetAll()
    {
        var records = await borrowRecordService.GetAllAsync();
        return Ok(records);
    }

    [HttpGet("member/{memberId:int}")]
    public async Task<ActionResult<List<BorrowRecordResponseDto>>> GetMemberHistory(int memberId)
    {
        var records = await borrowRecordService.GetMemberHistoryAsync(memberId);
        return Ok(records);
    }

    [HttpPost("borrow")]
    public async Task<ActionResult<BorrowRecordResponseDto>> Borrow([FromBody] BorrowBookRequestDto request)
    {
        var createdRecord = await borrowRecordService.BorrowBookAsync(request);
        // Borrowing creates a new borrow record.
        return StatusCode(StatusCodes.Status201Created, createdRecord);
    }

    [HttpPost("{borrowRecordId:int}/return")]
    public async Task<ActionResult<BorrowRecordResponseDto>> Return(int borrowRecordId)
    {
        var returnedRecord = await borrowRecordService.ReturnBookAsync(borrowRecordId);
        return Ok(returnedRecord);
    }
}
