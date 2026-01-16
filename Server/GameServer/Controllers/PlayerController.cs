using GameServer.Models.DTOs;
using GameServer.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        [EndpointSummary("플레이어 정보 요청")]
        public async Task<ActionResult<ApiResponse<PlayerInfoDto>>> GetPlayerInfo()
        {
            var accountId = (long)HttpContext.Items["AccountId"]!;
            var response = await _playerService.GetPlayerInfoAsync(accountId);

            return Ok(response);
        }

        [HttpPost("LevelUp")]
        [EndpointSummary("레벨업")]
        public async Task<ActionResult<ApiResponse<PlayerLevelUpResponseDto>>> LevelUp(PlayerLevelUpRequestDto requestDto)
        {
            var accountId = (long)HttpContext.Items["AccountId"]!;
            var response = await _playerService.PlayerLevelUp(accountId, requestDto);

            return Ok(response);
        }

        [HttpPost("GoldLevelUp")]
        [EndpointSummary("골드 레벨업")]
        public async Task<ActionResult<ApiResponse<GoldLevelUpResponseDto>>> GoldLevelUp(GoldLevelUpRequestDto requestDto)
        {
            var accountId = (long)HttpContext.Items["AccountId"]!;
            var response = await _playerService.GoldLevelUp(accountId, requestDto);

            return Ok(response);
        }
    }
}
