using FinanceApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    [HttpGet("error")]
    public IActionResult GetError()
    {
        throw new NotImplementedException("This endpoint is not yet implemented");
    }

    [HttpGet("notfound")]
    public IActionResult GetNotFound()
    {
        throw new NotFoundException("The requested account was not found");
    }

    [HttpGet("invalid")]
    public IActionResult GetInvalid()
    {
        throw new ArgumentException("Invalid account parameters provided");
    }

    [HttpGet]
    public IActionResult GetAccounts()
    {
        return Ok(new { Message = "This endpoint works correctly" });
    }
}