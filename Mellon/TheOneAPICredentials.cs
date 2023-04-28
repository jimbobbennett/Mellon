namespace Mellon;

/// <summary>
/// A wrapper class for credentials for accessing The One API using TheOneAPIClient.
///
/// Create this using your API key which you can get from https://the-one-api.dev.
/// </summary>
public class TheOneAPICredentials
{
    /// <summary>
    /// The API key for accessing The One API.
    /// </summary>
    public string ApiKey { get; private set; }

    /// <summary>
    /// Creates a new TheOneAPICredentials object.
    /// </summary>
    /// <param name="apiKey">The API key for accessing The One API.</param>
    public TheOneAPICredentials(string apiKey) => ApiKey = apiKey;
}