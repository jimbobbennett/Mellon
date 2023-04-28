/***
A test harness for playing with the Mellon SDK.

This SDK wraps The One API, a REST API to access Lord of the Rings books and movies data.
https://the-one-api.dev
***/

using System.Reflection;
using Mellon;
using Mellon.TestHarness.Configuration;
using Microsoft.Extensions.Configuration;

// Load the configuration to get the API key
#pragma warning disable CS8604
var configurationFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "configuration.json");
#pragma warning restore CS8604

var builder = new ConfigurationBuilder();
builder.AddJsonFile(configurationFilePath).AddUserSecrets<TheOneApiConfiguration>();
var configurationRoot = builder.Build();

// Get the API key from the configuration
var apiKey = configurationRoot.GetSection("TheOneApi")["apiKey"];

// Check the API key is not null
if (apiKey == null)
{
    Console.WriteLine("You must provide an API key for The One API. You can get one from https://the-one-api.dev");
    return;
}

// Create the client
var client = new TheOneAPIClient(new TheOneAPICredentials(apiKey), 1000);

var movie1 = await client.Movies.GetAsync("5cd95395de30eff6ebccde5d");
Console.WriteLine(movie1);

var quoteCount = 0;
var quotesForMovie = client.QuotesForMovie("5cd95395de30eff6ebccde5d");
await foreach(var quote in quotesForMovie)
{
    Console.WriteLine(quote);
    quoteCount++;
}

Console.WriteLine($"Quotes: {await quotesForMovie.CountAsync()}:{quoteCount}");

Console.WriteLine($"Movies: {await client.Movies.CountAsync()}");

await foreach(var movie in client.Movies)
{
    var m = await client.Movies.GetAsync(movie.Id);
    Console.WriteLine(m);
}

var quote1 = await client.Quotes.GetAsync("5cd96e05de30eff6ebcce7e9");
Console.WriteLine(quote1);

await foreach(var quote in client.Quotes)
{
    var q = await client.Quotes.GetAsync(quote.Id);
    // Console.WriteLine(q);
    quoteCount++;
}

Console.WriteLine($"Quotes: {await client.Quotes.CountAsync()}:{quoteCount}");