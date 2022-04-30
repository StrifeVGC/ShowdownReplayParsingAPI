namespace ShowdownReplayParser.Application.Models.ErrorModel
{
    public class Error
    {
        public Error(string message)
        {
            Message = message;
        }

        public Error(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }

        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
