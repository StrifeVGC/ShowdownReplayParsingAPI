using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShowdownReplayParser.Application.Models;
using ShowdownReplayParser.Application.Models.ErrorModel;
using ShowdownReplayParser.Application.Services.Contract;

namespace ShowdownReplayParser.Application.Controllers
{
    [ApiController]
    public class MatchController : Controller
    {
        private readonly IMatchService _matchService;
        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpPost]
        [Route("/details")]
        public IActionResult MatchDetails([FromBody] MatchReplayRequest request)
        {
            try
            {
                var result = _matchService.ParseMatchInformation(request);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                return new ObjectResult(JsonConvert.SerializeObject(new Error(ex.Message)));
            }
            
        }
    }
}
