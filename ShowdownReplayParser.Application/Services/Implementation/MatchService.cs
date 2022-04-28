using ShowdownReplayParser.Application.Common;
using ShowdownReplayParser.Application.Models;
using ShowdownReplayParser.Application.Services.Contract;

namespace ShowdownReplayParser.Application.Services.Implementation
{
    public class MatchService : IMatchService
    {
        public MatchService()
        {

        }

        public Match ParseTeamsAndPlayersFromLog(MatchReplayRequest request)
        {
            //find teams inside log
            var splitP1Team = Utils.getBetween(request.Log, Constants.PLAYERONEPOKEMON, Constants.PLAYERTWOPOKEMON);
            var splitP2Team = Utils.getBetween(request.Log, Constants.PLAYERTWOPOKEMON, Constants.TEAMPREVIEW);

            //remove \n
            var tempP1 = splitP1Team.Split(Constants.SLASHN);
            var tempP2 = splitP2Team.Split(Constants.SLASHN);

            var player1 = new Player { PlayerName = request.P1 };
            var player2 = new Player { PlayerName = request.P2 };

            //parse players from string obtained before
            player1.Team = ParsePlayerTeam(Constants.PLAYERONEPOKEMON, tempP1);
            player2.Team = ParsePlayerTeam(Constants.PLAYERTWOPOKEMON, tempP2);

            //find the winner in the log
            var findWinner = Utils.getBetween(request.Log, Constants.WINFIELD, Constants.SLASHN);

            var parsedMatch = new Match
            {
                PlayerOne = player1,
                PlayerTwo = player2,
                Winner = findWinner
            };

            parsedMatch = ParseBattleInfo(request.Log, parsedMatch);

            return parsedMatch;
        }


        public List<Pokemon> ParsePlayerTeam(string playerConstant, string[] teamString)
        {
            List<Pokemon> team = new List<Pokemon>();
            foreach (string s in teamString)
            {
                var pokemonLine = s.Replace(playerConstant, "");
                var pokemonInfo = pokemonLine.Split(",");

                if (pokemonInfo != null && !string.IsNullOrEmpty(pokemonInfo[0]))
                    team.Add(new Pokemon { Name = pokemonInfo[0] });
            }

            return team;
        }

        public Match ParseBattleInfo(string log, Match match)
        {
            var leadString = Utils.getBetween(log, Constants.STARTFIELD, Constants.ABILITYFIELD);
            var splitLeadString = leadString.Split(Constants.SLASHN);
            var recomposedString = string.Concat(splitLeadString);
            var pokemonInOrder = recomposedString.Split(Constants.SWITCHFIELD);

            foreach(string s in pokemonInOrder)
            {
                if(!string.IsNullOrEmpty(s))
                {
                    var noSpaces = s.Split(" ");
                    var pokemonName = noSpaces[1].Split("|")[1].Replace(",", "");

                    if (noSpaces[0] == Constants.PLAYERONEFIRSTPOKEMON || noSpaces[0] == Constants.PLAYERONESECONDPOKEMON)
                        match.PlayerOneLead.Add(new Pokemon { Name = pokemonName });

                    if (noSpaces[0] == Constants.PLAYERTWOFIRSTPOKEMON || noSpaces[0] == Constants.PLAYERTWOSECONDPOKEMON)
                        match.PlayerTwoLead.Add(new Pokemon { Name = pokemonName });
                }
                

            }

            return match;
        }
    }
}
