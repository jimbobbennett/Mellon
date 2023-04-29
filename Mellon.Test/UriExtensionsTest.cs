using FluentAssertions;

namespace Mellon.Test.Models;

[TestClass]
public class UriExtensionsTest
{
    [TestMethod]
    public void GivenOneParameter_WhenAddingToAUri_ThenTheUriHasTheParameter()
    {
        var uri = new Uri("https://the-one-api.dev/v2/movie");

        var result = uri.AddParameter("page", "2");

        result.AbsoluteUri.Should().Be("https://the-one-api.dev/v2/movie?page=2");
    }

    [TestMethod]
    public void GivenTwoParameters_WhenAddingToAUri_ThenTheUriHasBothParametera()
    {
        var uri = new Uri("https://the-one-api.dev/v2/movie");
        
        var result = uri.AddParameter("page", "2").AddParameter("limit", "10");

        result.AbsoluteUri.Should().Be("https://the-one-api.dev/v2/movie?page=2&limit=10");
    }
}