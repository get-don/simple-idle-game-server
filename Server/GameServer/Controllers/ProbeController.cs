using GameServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [Route("")]
    [ApiController]
    public class ProbeController : ControllerBase
    {
        private readonly AppState _appState;
        public ProbeController(AppState appState) => _appState = appState;

        [AllowAnonymous]
        [HttpGet("/startup")]
        [EndpointSummary("k8s의 Startup Probe용 Path")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public IActionResult StartUp() => _appState.StartUp ? Ok() : StatusCode(503);

        [AllowAnonymous]
        [HttpGet("/ready")]
        [EndpointSummary("k8s의 Readiness Probe용 Path")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public IActionResult Readiness() => _appState.Ready ? Ok() : StatusCode(503);

        [AllowAnonymous]
        [HttpGet("healthz")]
        [EndpointSummary("k8s의 Liveness Probe용 Path")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Liveness() => Ok();
    }
}
