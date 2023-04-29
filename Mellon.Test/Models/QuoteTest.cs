using System.Text.Json;
using FluentAssertions;
using Mellon.Models;

namespace Mellon.Test.Models;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

[TestClass]
public class QuoteTest
{
    [TestMethod]
    public void GivenCompleteQuoteJson_WhenDeserializing_ThenQuoteIsCreatedWithAllFields()
    {
        var json = @"{
            ""_id"": ""5cd96e05de30eff6ebccebe7"",
            ""dialog"": ""Never thought I'd die fighting side by side with an elf."",
            ""movie"": ""5cd95395de30eff6ebccde5d"",
            ""character"": ""5cd99d4bde30eff6ebccfd23"",
            ""id"": ""5cd96e05de30eff6ebccebe7""
            }";

        var quote = JsonSerializer.Deserialize<Quote>(json);

        quote.Should().NotBeNull();
        quote.Id.Should().Be("5cd96e05de30eff6ebccebe7");
        quote.Dialog.Should().Be("Never thought I'd die fighting side by side with an elf.");
        quote.MovieId.Should().Be("5cd95395de30eff6ebccde5d");
        quote.CharacterId.Should().Be("5cd99d4bde30eff6ebccfd23");
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void GivenIncompleteQuoteJson_WhenDeserializing_ThenJsonExceptionIsThrown()
    {
        var json = @"{
            ""_id"": ""5cd96e05de30eff6ebccebe7"",
            ""movie"": ""5cd95395de30eff6ebccde5d"",
            ""character"": ""5cd99d4bde30eff6ebccfd23"",
            ""id"": ""5cd96e05de30eff6ebccebe7""
            }";

        var quote = JsonSerializer.Deserialize<Quote>(json);
    }

    [TestMethod]
    public void GivenQuote_WhenCallingToString_ThenStringIsReturnedWithDialogAndId()
    {
        var json = @"{
            ""_id"": ""5cd96e05de30eff6ebccebe7"",
            ""dialog"": ""Never thought I'd die fighting side by side with an elf."",
            ""movie"": ""5cd95395de30eff6ebccde5d"",
            ""character"": ""5cd99d4bde30eff6ebccfd23"",
            ""id"": ""5cd96e05de30eff6ebccebe7""
            }";

        var quote = JsonSerializer.Deserialize<Quote>(json);

        quote.Should().NotBeNull();
        quote.ToString().Should().Be("Never thought I'd die fighting side by side with an elf. (Id: 5cd96e05de30eff6ebccebe7)");
    }
}