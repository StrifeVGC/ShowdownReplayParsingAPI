using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShowdownReplayParser.Application.Models;
using ShowdownReplayParser.Application.Models.ErrorModel;
using ShowdownReplayParser.Application.Services.Contract;

namespace ShowdownReplayParser.Application.Controllers
{
    [ApiController]
    public class BattleController : Controller
    {
        private readonly IBattleService _matchService;
        public BattleController(IBattleService matchService)
        {
            _matchService = matchService;
        }

        [HttpPost]
        [Route("/details")]
        public IActionResult MatchDetails([FromBody] BattleReplayRequest request)
        {
            try
            {
                var result = _matchService.ParseBattleInformation(request);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                return new ObjectResult(JsonConvert.SerializeObject(new Error(ex.Message)));
            }
            
        }
    }
}
