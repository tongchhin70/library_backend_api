namespace library_backend_api.Exceptions;

// Thrown when input passes model validation but fails business validation.
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}
