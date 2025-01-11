namespace CpmConverterCli.Exceptions;

public class FriendlyException : System.Exception
{
    public FriendlyException()
    {
    }

    public FriendlyException(string message) : base(message)
    {
    }

    public FriendlyException(string message, System.Exception inner) : base(message, inner)
    {
    }
}