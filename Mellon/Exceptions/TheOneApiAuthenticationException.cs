namespace Mellon.Exceptions;

/// <summary>
/// An exception thrown when The One API authentication fails
/// </summary>
public class TheOneApiAuthenticationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TheOneApiAuthenticationException"/> class.
    /// </summary>
    public TheOneApiAuthenticationException() : base("The One API authentication failed. Check your API key.")
    {
    }
}