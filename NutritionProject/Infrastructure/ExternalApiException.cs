namespace Infrastructure
{
    public class ExternalApiException : Exception
    {
        public ExternalApiException(string message, Exception? inner = null)
            : base(message, inner) { }
    }

}
