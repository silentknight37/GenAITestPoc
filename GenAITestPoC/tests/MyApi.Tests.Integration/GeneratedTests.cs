using System.Net;
using System.Net.Http.Json;
using Xunit;
using MyApi.Models;

namespace MyApi.Tests.Integration;

public class GeneratedTests : IClassFixture<CustomWebAppFactory>, IDisposable
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _client;
    public GeneratedTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        DbReset.Reset(_factory);
        _client = factory.CreateClient();
    }
    public void Dispose()
    {
         _client.Dispose();
    }

    [Fact]
    public async Task TC_REG_001_Valid_registration_returns_201()
    {
        var res = await _client.PostAsJsonAsync("/api/users/register", new RegisterRequest("ok@test.com", "Password1", 22));
        Assert.Equal((HttpStatusCode)201, res.StatusCode);
    }

    [Fact]
    public async Task TC_REG_002_Weak_password_returns_400()
    {
        var res = await _client.PostAsJsonAsync("/api/users/register", new RegisterRequest("weak@test.com", "pass", 22));
        Assert.Equal((HttpStatusCode)400, res.StatusCode);
    }

    [Fact]
    public async Task TC_REG_003_Underage_returns_400()
    {
        var res = await _client.PostAsJsonAsync("/api/users/register", new RegisterRequest("kid@test.com", "Password1", 17));
        Assert.Equal((HttpStatusCode)400, res.StatusCode);
    }

    [Fact]
    public async Task TC_REG_004_Duplicate_email_returns_409()
    {
        await _client.PostAsJsonAsync("/api/users/register", new RegisterRequest("dup@test.com", "Password1", 22));

        var res = await _client.PostAsJsonAsync("/api/users/register", new RegisterRequest("dup@test.com", "Password1", 22));
        Assert.Equal((HttpStatusCode)409, res.StatusCode);
    }

}
