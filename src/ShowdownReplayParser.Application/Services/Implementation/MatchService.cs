using ShowdownReplayParser.Application.Common;
using ShowdownReplayParser.Application.Models;
using ShowdownReplayParser.Application.Services.Contract;
using System.Text.RegularExpressions;

namespace ShowdownReplayParser.Application.Services.Implementation
{
    public class BattleService : IBattleService
    {
        private readonly ILogger<BattleService> _logger;
        public BattleService(ILogger<BattleService> logger)
        {
            _logger = logger;
        }

        public Battle ParseBattleInformation(BattleReplayRequest request)
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

                var parsedBattle = new Battle
                {
                    PlayerOne = player1,
                    PlayerTwo = player2,
                    Winner = findWinner
                };

                parsedBattle = ParseBattleInfo(request.Log, parsedBattle);

                return parsedBattle;
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

        private Battle ParseBattleInfo(string log, Battle battle)
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
                        battle.PlayerOneLead.Add(new Pokemon { Name = pokemonName });

                    if (BelongsToSecondPlayer(noSpaces[0]))
                        battle.PlayerTwoLead.Add(new Pokemon { Name = pokemonName });
                }
                

            }

            //check for turn 1 dynamaxes
            //TODO: Implement nickname checking
            CheckTurnOneDynamax(log, battle);

            //parse moves
            var BattleString = Utils.GetBetween(log, Constants.MOVEFIELD, Constants.WINFIELD);
            var tempBattleString = BattleString.Split(Constants.SLASHN);
            var recomposedBattle = string.Concat(tempBattleString);
            var movesString = recomposedBattle.Split(Constants.MOVEFIELD);

            

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
                        var pokemon = battle.PlayerOne.Team.FirstOrDefault(x => x.Name == dynamaxInfo.Split(" ")[1]);
                        if(pokemon != null)
                        {
                            SetDynamaxPokemon(pokemon);
                        }             
                    }

                    if (BelongsToSecondPlayer(dynamaxInfo.Split(" ")[0]))
                    {
                        var pokemon = battle.PlayerTwo.Team.FirstOrDefault(x => x.Name == dynamaxInfo.Split(" ")[1]);
                        if (pokemon != null)
                        {
                            SetDynamaxPokemon(pokemon);
                        }
                    }

                }

                if (BelongsToFirstPlayer(tempMoves[0].Split(" ")[0]))
                {
                    var pokemon = battle.PlayerOne.Team.FirstOrDefault(x => x.Name == pokemonName);
                    if(pokemon != null)
                    {
                        AddMoveToPokemon(pokemon, moveName);
                    }              
                }

                if (BelongsToSecondPlayer(tempMoves[0].Split(" ")[0]))
                {
                    var pokemon = battle.PlayerTwo.Team.FirstOrDefault(x => x.Name == pokemonName);
                    if (pokemon != null)
                    {
                        AddMoveToPokemon(pokemon, moveName);
                    }
                }
            }

            return battle;
        }

        private bool BelongsToFirstPlayer(string playerConstant)
        {
            return (playerConstant == Constants.PLAYERONEFIRSTPOKEMON || playerConstant == Constants.PLAYERONESECONDPOKEMON);
        }

        private bool BelongsToSecondPlayer(string playerConstant)
        {
            return (playerConstant == Constants.PLAYERTWOFIRSTPOKEMON || playerConstant == Constants.PLAYERTWOSECONDPOKEMON);
        }

        private void SetDynamaxPokemon(Pokemon pokemon)
        {
            
            if (pokemon != null)
            {
                pokemon.HasDynamaxed = true;
                
            }
        }

        private void AddMoveToPokemon(Pokemon pokemon, string moveName)
        {
            if (pokemon != null && !pokemon.Moves.Contains(moveName) && !moveName.StartsWith(Constants.MAX_PREFIX))
            {
                pokemon.Moves.Add(moveName);
            }

            else if (pokemon != null && !pokemon.MaxMoves.Contains(moveName) && moveName.StartsWith(Constants.MAX_PREFIX))
            {
                pokemon.MaxMoves.Add(moveName);
            }
        }

        private void CheckTurnOneDynamax(string log, Battle battle)
        { 

            var firstTurnDynamaxesString = Utils.GetBetween(log, Constants.MINUSSTART, Constants.MOVEFIELD);
            var startofTurnString = string.Concat(firstTurnDynamaxesString.Split(Constants.SLASHN));

            //two pokemon dynamaxed turn one
            if (Regex.Matches(startofTurnString, Constants.DYNAMAXFIELD).Count == 2)
            {
                var splitDynamaxes = firstTurnDynamaxesString.Split(Constants.MINUSSTART);

                if(splitDynamaxes!= null && splitDynamaxes.Any())
                {
                    foreach(string s in splitDynamaxes)
                    {
                        SetTurnOneDynamax(battle, s);
                    }
                }
            }

            //one pokemon dynamaxed turn one
            else if(Regex.Matches(startofTurnString, Constants.DYNAMAXFIELD).Count == 1)
            {
                SetTurnOneDynamax(battle, firstTurnDynamaxesString);
            }
        }

        private void SetTurnOneDynamax(Battle battle, string toParse)
        {
            var splitMaxString = toParse.Split("|");
            bool isGmax = toParse.Contains(Constants.GMAX);

            string owner = splitMaxString[0].Split(" ")[0];
            string pokemonName = splitMaxString[0].Split(" ")[1];
            if (BelongsToFirstPlayer(owner))
            {
                var pokemon = battle.PlayerOne.Team.FirstOrDefault(x => x.Name == pokemonName);
                if (pokemon != null)
                {
                    pokemon.HasDynamaxed = true;
                    pokemon.IsGmax = isGmax;
                }

            }

            if (BelongsToSecondPlayer(owner))
            {
                var pokemon = battle.PlayerTwo.Team.FirstOrDefault(x => x.Name == pokemonName);
                if (pokemon != null)
                {
                    pokemon.HasDynamaxed = true;
                    pokemon.IsGmax = isGmax;
                }

            }
        }

        #endregion
    }
}
