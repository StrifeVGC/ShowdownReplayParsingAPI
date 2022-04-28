namespace ShowdownReplayParser.Application.Models
{
    public class MatchReplayRequest
    {
        public string Id { get; set; }
        public string P1 { get; set; }
        public string P2 { get; set; }
        public string Format { get; set; }
        public string Log { get; set; }
        public int UploadTime { get; set; }
        public int Views { get; set; }
        public string P1Id { get; set; }
        public string P2Id { get; set; }
        public string FormatId { get; set; }
        public int Rating { get; set; }
        public int Private { get; set; }
    }
}
