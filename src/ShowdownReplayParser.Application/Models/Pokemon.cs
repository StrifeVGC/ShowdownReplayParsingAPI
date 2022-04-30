namespace ShowdownReplayParser.Application.Models
{
    public class Pokemon
    {
        public Pokemon()
        {
            Moves = new List<string>();
            MaxMoves = new List<string>();
        }

        public string? Name { get; set; }
        public string? Forme { get; set; }
        public List<string> Moves { get; set; }
        public List<string> MaxMoves { get; set; }
        public bool HasDynamaxed { get; set; }
        public bool IsGmax { get; set; }
    }
}
