namespace library_backend_api.Exceptions;

// Thrown when a requested resource does not exist.
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
