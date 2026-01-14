using GameServer.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        [HttpGet("PlayerInfo")]
        public IActionResult GetPlayerInfo()
        {
            return Ok();
        }

        [HttpPost("StageBegin")]
        public IActionResult BeginStage(StageBeginDto stageBeginDto)
        {
            return Ok();
        }

        [HttpPost("StageClear")]
        public IActionResult ClearStage(StageClearDto stageClearDto)
        {
            return Ok();
        }

        [HttpPost("LevelUp")]
        public IActionResult LevelUp()
        {
            return Ok();
        }

        [HttpPost("GoldLevelUp")]
        public IActionResult GoldLevelUp()
        {
            return Ok();
        }
    }
}
