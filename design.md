# Design of the Mellon SDK

This SDK was designed with the following in mind:

- Caching to reduce network calls
- Easy to add new endpoint
- Standard .NET patterns
- Enumerables of objects rather than an object graph

## Caching

Data available from The One Api is unlikely to change unless a new movie is released. The data is very unlikely to change in the lifetime of running an app using the SDK. Therefor the SDK caches all data inside the client. If there is ever a need to refresh the data, a new client object can created to reload the data from the API.

## Add new endpoints

The internals of the SDK use an enumerator class, `TheOneApiEnumerator<T>` that implements `IAsyncEnumerator`. This is a generic class that can be created using different models and endpoints, so can be extended to load new types for new endpoints.

All the endpoints of The One Api return data in the same way, using a response object with one or more entries in a `docs` array, along with pagination details. The enumerator handles all the pagination by loading one page at a time using a defined page size, then loading more pages if the enumerator moves past the last loaded entry. Each enumerator instance is typed using a model class that wraps the different types of data in the `docs` array in the response, and this type needs to derive from `TheOneApiModelBase`, a base class with an Id.

To handle an endpoint, a model is needed that can deserialize the JSON for the items in the `docs` array, along with the endpoint. These are passed to the enumerator when it is constructed. To handle a new endpoint, create a new model, and a new instance of the enumerator, exposing it through the client object.

For example, for movies there is a model:

```csharp
public class Movie : TheOneApiModelBase
{
    [JsonPropertyName("name")]
    [JsonRequired]
    [JsonInclude]
    public string Name { get; private set; }

    [JsonPropertyName("runtimeInMinutes")]
    [JsonRequired]
    [JsonInclude]
    public int RuntimeInMinutes { get; private set; }
    ...
}
```

This is then used to create an enumerator for Movies:

```csharp
public class Movies : TheOneApiEnumerable<Movie>
{
    internal Movies(string apiKey, int pageSize) : base("movie", apiKey, pageSize)
    {}
}
```

To add books, you could create the relevant model and enumerator:

```csharp
public class Book : TheOneApiModelBase
{
    [JsonPropertyName("name")]
    [JsonRequired]
    [JsonInclude]
    public string Name { get; private set; }

    [JsonConstructor]
    public Movie(string id, string name) : base(id) => (Name) = (name);
}

public class Books : TheOneApiEnumerable<Book>
{
    internal Books(string apiKey, int pageSize) : base("book", apiKey, pageSize)
    {}
}
```

## .NET patterns

The SDK uses async enumerators, with helper functions to retrieve by Id and count the items. Each enumerator implements `IAsyncEnumerator<T>` so can be enumerated like any async enumerable:

```csharp
await foreach (var m in client.Movies)
{
    // do something
}
```

This also allows you to use `System.Linq.Async`:

```csharp
var first = await client.Movies.FirstAsync();
```

## Object enumerables rather than object graphs

To keep the implementation simpler, the data from API is in enumerables rather than a graph. This avoids the complexities of building and navigating a graph as it feels unlikely that an application built from the SDK would navigate through a graph from movie to quote to movie to quote and so on.

The graph-like navigation is implemented by helper methods - either getting a movie or quote by Id, or getting all quotes for a movie:

```csharp
var quote = await client.Quotes.FirstAsync();
var movie = await client.Movies.GetAsync(quote.MovieId);
```
