using ShowdownReplayParser.Application.Models;

namespace ShowdownReplayParser.Application.Services.Contract
{
    public interface IMatchService
    {
        public Match ParseTeamsAndPlayersFromLog(MatchReplayRequest request);
        public List<Pokemon> ParsePlayerTeam(string playerConstant, string[] teamString);
    }
}
