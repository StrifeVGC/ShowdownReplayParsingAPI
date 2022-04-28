using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShowdownReplayParser.Application.Models;
using ShowdownReplayParser.Application.Services.Contract;

namespace ShowdownReplayParser.Application.Controllers
{
    [ApiController]
    public class MatchController : Controller
    {
        IMatchService _matchService;
        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpPost]
        [Route("/details")]
        public IActionResult MatchDetails([FromBody] MatchReplayRequest request)
        {
            var result = _matchService.ParseTeamsAndPlayersFromLog(request);
            return Ok(JsonConvert.SerializeObject(result));
        }
    }
}
