using ShowdownReplayParser.Application.Common;
using ShowdownReplayParser.Application.Models;
using ShowdownReplayParser.Application.Services.Contract;

namespace ShowdownReplayParser.Application.Services.Implementation
{
    public class MatchService : IMatchService
    {
        private readonly ILogger<MatchService> _logger;
        public MatchService(ILogger<MatchService> logger)
        {
            _logger = logger;
        }

        public Match ParseMatchInformation(MatchReplayRequest request)
        {
            try
            {
                //find teams inside log
                var splitP1Team = Utils.GetBetween(request.Log, Constants.PLAYERONEPOKEMON, Constants.PLAYERTWOPOKEMON);
                var splitP2Team = Utils.GetBetween(request.Log, Constants.PLAYERTWOPOKEMON, Constants.TEAMPREVIEW);

                //remove \n
                var tempP1 = splitP1Team.Split(Constants.SLASHN);
                var tempP2 = splitP2Team.Split(Constants.SLASHN);

                var player1 = new Player { PlayerName = request.P1 };
                var player2 = new Player { PlayerName = request.P2 };

                //parse players from string obtained before
                player1.Team = ParsePlayerTeam(Constants.PLAYERONEPOKEMON, tempP1);
                player2.Team = ParsePlayerTeam(Constants.PLAYERTWOPOKEMON, tempP2);

                //find the winner in the log
                var findWinner = Utils.GetBetween(request.Log, Constants.WINFIELD, Constants.SLASHN);

                var parsedMatch = new Match
                {
                    PlayerOne = player1,
                    PlayerTwo = player2,
                    Winner = findWinner
                };

                parsedMatch = ParseBattleInfo(request.Log, parsedMatch);

                return parsedMatch;
            }
            catch(Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                throw;
            }
        }

        #region private methods
        private List<Pokemon> ParsePlayerTeam(string playerConstant, string[] teamString)
        {
            List<Pokemon> team = new List<Pokemon>();
            foreach (string s in teamString)
            {
                var pokemonLine = s.Replace(playerConstant, "");
                var pokemonInfo = pokemonLine.Split(",");

                if (pokemonInfo != null && !string.IsNullOrEmpty(pokemonInfo[0]))
                {
                    if(pokemonInfo[0].Contains("-"))
                    {
                        team.Add(new Pokemon { Name = pokemonInfo[0].Split("-")[0], Forme = pokemonInfo[0].Split("-")[1] });
                    }
                    else 
                    { 
                        team.Add(new Pokemon { Name = pokemonInfo[0] }); 
                    }
                }
                    
            }

            return team;
        }

        private Match ParseBattleInfo(string log, Match match)
        {
            //Parse leads
            var leadString = Utils.GetBetween(log, Constants.STARTFIELD, Constants.ABILITYFIELD);
            var splitLeadString = leadString.Split(Constants.SLASHN);
            var recomposedString = string.Concat(splitLeadString);
            var pokemonInOrder = recomposedString.Split(Constants.SWITCHFIELD);

            foreach(string s in pokemonInOrder)
            {
                if(!string.IsNullOrEmpty(s))
                {
                    var noSpaces = s.Split(" ");
                    var pokemonName = noSpaces[1].Split("|")[1].Replace(",", "");

                    if (BelongsToFirstPlayer(noSpaces[0]))
                        match.PlayerOneLead.Add(new Pokemon { Name = pokemonName });

                    if (BelongsToSecondPlayer(noSpaces[0]))
                        match.PlayerTwoLead.Add(new Pokemon { Name = pokemonName });
                }
                

            }

            //parse moves
            var matchString = Utils.GetBetween(log, Constants.MOVEFIELD, Constants.WINFIELD);
            var movesString = matchString.Split(Constants.MOVEFIELD);

            foreach(string s in movesString)
            {
                var tempMoves = s.Split("|");
                var pokemonName = tempMoves[0].Split(" ")[1];
                var moveName = tempMoves[1];

                //parse if a pokemon dynamaxed
                if(tempMoves.Contains(Constants.MINUSSTARTFIELD) && tempMoves.Contains(Constants.DYNAMAXFIELD))
                {
                    var dynamaxInfo = tempMoves[Array.IndexOf(tempMoves, Constants.MINUSSTARTFIELD) + 1];

                    if(BelongsToFirstPlayer(dynamaxInfo.Split(" ")[0]))
                    {
                        var pokemon = match.PlayerOne.Team.FirstOrDefault(x => x.Name == dynamaxInfo.Split(" ")[1]);
                        if(pokemon != null)
                        {
                            match = SetDynamaxPokemon(match, pokemon);
                        }             
                    }

                    if (BelongsToSecondPlayer(dynamaxInfo.Split(" ")[0]))
                    {
                        var pokemon = match.PlayerTwo.Team.FirstOrDefault(x => x.Name == dynamaxInfo.Split(" ")[1]);
                        if (pokemon != null)
                        {
                            match = SetDynamaxPokemon(match, pokemon);
                        }
                    }

                }

                if (BelongsToFirstPlayer(tempMoves[0].Split(" ")[0]))
                {
                    var pokemon = match.PlayerOne.Team.FirstOrDefault(x => x.Name == pokemonName);
                    if(pokemon != null)
                    {
                        match = AddMoveToPokemon(match, pokemon, moveName);
                    }              
                }

                if (BelongsToSecondPlayer(tempMoves[0].Split(" ")[0]))
                {
                    var pokemon = match.PlayerTwo.Team.FirstOrDefault(x => x.Name == pokemonName);
                    if (pokemon != null)
                    {
                        match = AddMoveToPokemon(match, pokemon, moveName);
                    }
                }
            }

            return match;
        }

        private bool BelongsToFirstPlayer(string playerConstant)
        {
            return (playerConstant == Constants.PLAYERONEFIRSTPOKEMON || playerConstant == Constants.PLAYERONESECONDPOKEMON);
        }

        private bool BelongsToSecondPlayer(string playerConstant)
        {
            return (playerConstant == Constants.PLAYERTWOFIRSTPOKEMON || playerConstant == Constants.PLAYERTWOSECONDPOKEMON);
        }

        private Match SetDynamaxPokemon(Match match, Pokemon pokemon)
        {
            
            if (pokemon != null)
            {
                pokemon.HasDynamaxed = true;
            }
            return match;
        }

        private Match AddMoveToPokemon(Match match, Pokemon pokemon, string moveName)
        {
            if (pokemon != null && !pokemon.Moves.Contains(moveName) && !moveName.StartsWith(Constants.MAX_PREFIX))
            {
                pokemon.Moves.Add(moveName);
            }

            else if (pokemon != null && !pokemon.MaxMoves.Contains(moveName) && moveName.StartsWith(Constants.MAX_PREFIX))
            {
                    pokemon.MaxMoves.Add(moveName);
            }

            return match;
        }
        #endregion
    }
}
