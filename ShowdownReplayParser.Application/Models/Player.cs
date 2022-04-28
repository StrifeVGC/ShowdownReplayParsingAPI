namespace ShowdownReplayParser.Application.Models
{
    public class Player
    {
        public Player()
        {
            Team = new List<Pokemon>();
        }

        public string PlayerName { get; set; }
        public List<Pokemon> Team { get; set; }
    }
}
