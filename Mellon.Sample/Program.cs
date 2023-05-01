using System.Reflection;
using Mellon;
using Mellon.Exceptions;
using Mellon.Sample.Configuration;
using Microsoft.Extensions.Configuration;

// Load the API key from a configuration file or user secrets
#pragma warning disable CS8604
var configurationFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "configuration.json");
#pragma warning restore CS8604

var builder = new ConfigurationBuilder();
builder.AddJsonFile(configurationFilePath).AddUserSecrets<TheOneApiConfiguration>();
var configurationRoot = builder.Build();

// Get the API key from the configuration
var apiKey = configurationRoot.GetSection("TheOneApi")["apiKey"];

// Create the API client
var client = new TheOneAPIClient(new TheOneAPICredentials(apiKey!));

// Ask the user what movie they want a quote from
Console.WriteLine("What movie do you want a random quote from?");

// Get the selection
var movieNumber = 1;
await foreach (var movie in client.Movies)
{
    Console.WriteLine($"{movieNumber}: {movie.Name}");
    movieNumber++;
}

// Get the movie
var movieSelection = Console.ReadLine();

// Get the movie index
int movieIndex;

if (!int.TryParse(movieSelection, out movieIndex))
{
    Console.WriteLine("Invalid selection");
    return;
}

Console.WriteLine($"You selected {movieIndex}");

// Get the movie
var movieToQuote = await client.Movies.ElementAtAsync(movieIndex - 1);

// Get the quotes
var quotes = client.QuotesForMovie(movieToQuote.Id);

// Get a random quote
var random = new Random();
var quote = await quotes.ElementAtAsync(random.Next(0, await quotes.CountAsync()));

// Display the quote
Console.WriteLine($"{movieToQuote.Name}: {quote.Dialog}");
