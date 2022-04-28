namespace ShowdownReplayParser.Application.Models
{
    public class Match
    {
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }

        public string Format { get; set; }
        public string Winner { get; set; }
        public string Replay { get; set; }
    }
}
