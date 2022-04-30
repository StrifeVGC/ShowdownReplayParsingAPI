using ShowdownReplayParser.Application.Models;

namespace ShowdownReplayParser.Application.Services.Contract
{
    public interface IBattleService
    {
        public Battle ParseBattleInformation(BattleReplayRequest request);
    }
}
