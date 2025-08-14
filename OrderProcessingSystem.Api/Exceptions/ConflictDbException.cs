namespace OrderProcessingSystem.Api.Exceptions
{
    public class ConflictDbException : Exception
    {
        public ConflictDbException(string message) : base(message)
        {
        }
        public ConflictDbException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
