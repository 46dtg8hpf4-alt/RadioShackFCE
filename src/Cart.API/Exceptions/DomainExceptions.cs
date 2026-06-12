namespace Cart.API.Exceptions
{
    public class NotFoundException(string errorCode, string message) : Exception(message)
    {
        public string ErrorCode { get; } = errorCode;
    }

    public class BusinessRuleException(string errorCode, string message) : Exception(message)
    {
        public string ErrorCode { get; } = errorCode;
    }

    public class ValidationException(string errorCode, string message) : Exception(message)
    {
        public string ErrorCode { get; } = errorCode;
    }
}
