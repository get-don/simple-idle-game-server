using GameServer.Models.DTOs;
using GameServer.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StageController : ControllerBase
    {
        private readonly IStageService _stageService;

        public StageController(IStageService stageService)
        {
            _stageService = stageService;
        }

        [HttpPost("Clear")]
        [EndpointSummary("스테이지 클리어")]
        public async Task<ActionResult<ApiResponse<StageClearResponseDto>>> StageClear(StageClearRequestDto requestDto)
        {
            var accountId = (long)HttpContext.Items["AccountId"]!;
            var response = await _stageService.StageClearAsync(accountId, requestDto);

            return Ok(response);            
        }
    }
}
