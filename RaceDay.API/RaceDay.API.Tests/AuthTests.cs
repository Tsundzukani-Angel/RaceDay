using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace RaceDay.API.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();

    // Register 
    [Fact]
    public async Task Register_NewParticipant_Returns201()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "Test",
            lastName = "Participant",
            email = $"p_{Guid.NewGuid():N}@raceday.co.za",
            password = "Pass123!",
            role = "Participant",
            phoneNumber = "0810000000"
        });

        res.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var email = $"dup_{Guid.NewGuid():N}@raceday.co.za";
        var payload = new
        {
            firstName = "Dup",
            lastName = "User",
            email,
            password = "Pass123!",
            role = "Participant",
            phoneNumber = "0810000001"
        };

        await _client.PostAsJsonAsync("/api/auth/register", payload);
        var second = await _client.PostAsJsonAsync("/api/auth/register", payload);

        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_InvalidRole_Returns400()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "Bad",
            lastName = "Role",
            email = $"bad_{Guid.NewGuid():N}@raceday.co.za",
            password = "Pass123!",
            role = "Admin"
        });

        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Login
    [Fact]
    public async Task Login_ValidCredentials_Returns200()
    {
        var email = $"login_{Guid.NewGuid():N}@raceday.co.za";
        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "Login",
            lastName = "User",
            email,
            password = "Pass123!",
            role = "Participant"
        });

        var res = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password = "Pass123!"
        });

        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "noone@nowhere.com",
            password = "wrong"
        });

        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
