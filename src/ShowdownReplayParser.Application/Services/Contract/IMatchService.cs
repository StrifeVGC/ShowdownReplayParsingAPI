using ShowdownReplayParser.Application.Models;

namespace ShowdownReplayParser.Application.Services.Contract
{
    public interface IMatchService
    {
        public Match ParseMatchInformation(MatchReplayRequest request);
    }
}
