namespace Mellon.Exceptions;

/// <summary>
/// A generic exception type for failed The One API requests
/// </summary>
public class TheOneApiRequestException: Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TheOneApiRequestException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TheOneApiRequestException(string message) : base(message)
    {
    }
}