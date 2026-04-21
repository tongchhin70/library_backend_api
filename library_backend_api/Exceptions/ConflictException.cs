namespace library_backend_api.Exceptions;

// Thrown when the request conflicts with current system state.
public class ConflictException(string message) : Exception(message);
