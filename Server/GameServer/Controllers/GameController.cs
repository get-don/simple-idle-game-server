using GameServer.Models.DTOs;
using GameServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IPlayerRepository _repository;

        public GameController(IPlayerRepository repo)
        {
            _repository = repo;
        }

        [HttpGet("PlayerInfo")]
        [EndpointSummary("플레이어 정보 요청")]
        public async Task<ActionResult<ApiResponse<PlayerInfoDto>>> GetPlayerInfo()
        {
            var response = new ApiResponse<PlayerInfoDto>();

            var accountId = (long)HttpContext.Items["AccountId"]!;

            Console.WriteLine($"[GameController.GetPlayerInfo] AccountId: {accountId}");

            var playerInfo = await _repository.GetPlayerByAccountIdAsync(accountId);
            if(playerInfo is null)
            {
                response.Ok = false;
                response.ErrorCode = ErrorCode.NotExistsPlayer;                
                return Ok(response);
            }

            response.Result = new PlayerInfoDto()
            {
                Level = playerInfo.Level,
                GoldLevel = playerInfo.GoldLevel,
                Stage = playerInfo.Stage,
                Gold = playerInfo.Gold
            };

            Console.WriteLine($"[GameController.GetPlayerInfo] AccountId: {accountId}, Level: {playerInfo.Level}, GoldLevel: {playerInfo.GoldLevel}, Stage: {playerInfo.Stage}, Gold: {playerInfo.Gold}");

            return Ok(response);
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
