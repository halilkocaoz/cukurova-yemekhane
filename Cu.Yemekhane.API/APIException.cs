namespace Cu.Yemekhane.API;

public class ApiException : Exception
{
    public ApiException(string message) => Message = message;

    public override string Message { get; }
}