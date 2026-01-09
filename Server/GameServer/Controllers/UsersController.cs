using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpGet("Info")]
    public IActionResult GetUserInfo()
    {        
        return Ok();
    }
}
