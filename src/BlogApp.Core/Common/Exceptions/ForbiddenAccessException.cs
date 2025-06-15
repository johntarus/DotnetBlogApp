namespace BlogApp.Core.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("You do not have access to this resource"){}
    public ForbiddenAccessException(string message) : base(message){}
    public ForbiddenAccessException(string message, Exception inner) : base(message, inner){}
    
}