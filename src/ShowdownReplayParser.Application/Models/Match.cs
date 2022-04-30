namespace ShowdownReplayParser.Application.Models
{
    public class Match
    {
        public Match()
        {
            PlayerOneLead = new List<Pokemon>();
            PlayerTwoLead = new List<Pokemon>();
        }

        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public List<Pokemon> PlayerOneLead { get; set; }
        public List<Pokemon> PlayerTwoLead { get; set; }

        public string Format { get; set; }
        public string Winner { get; set; }
        public string Replay { get; set; }
    }
}
