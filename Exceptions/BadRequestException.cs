namespace library_backend_api.Exceptions;

// Thrown when input passes model validation but fails business validation.
public class BadRequestException(string message) : Exception(message);
