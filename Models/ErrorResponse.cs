namespace KalumManagement.Models
{
    public class ErrorResponse
    {
        public string ErrorType { get; set; }
        public int HttpStatusCode { get; set; }
        public string Message { get; set; }
    }
}