namespace Core.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public object Errors { get; }

    public ApiException(string message, int statusCode = 500, object errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}