using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RaceDay.API.Tests;

public class RoleAccessTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public RoleAccessTests(CustomWebApplicationFactory factory) => _factory = factory;

    // A client that keeps cookies, needed for session auth
    private HttpClient NewCookieClient() =>
        _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            AllowAutoRedirect = false
        });

    private static object NewEventBody() => new
    {
        name = "Test Marathon",
        description = "A test event",
        eventDate = DateTime.UtcNow.AddDays(30),
        location = "Durban",
        distance = 42.2,
        eventType = "Run",
        maxParticipants = 500,
        registrationDeadline = DateTime.UtcNow.AddDays(20),
        status = "Open"
    };

    // Unauthenticated 
    [Fact]
    public async Task CreateEvent_Unauthenticated_Returns403()
    {
        var client = NewCookieClient();
        var res = await client.PostAsJsonAsync("/api/events", NewEventBody());

        res.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetEvents_Unauthenticated_Returns200()
    {
        var client = NewCookieClient();
        var res = await client.GetAsync("/api/events");

        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Wrong role 
    [Fact]
    public async Task CreateEvent_AsParticipant_Returns403()
    {
        var client = NewCookieClient();
        var email = $"part_{Guid.NewGuid():N}@raceday.co.za";

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "P",
            lastName = "User",
            email,
            password = "Pass123!",
            role = "Participant"
        });
        await client.PostAsJsonAsync("/api/auth/login", new { email, password = "Pass123!" });

        var res = await client.PostAsJsonAsync("/api/events", NewEventBody());

        res.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // Correct role 
    [Fact]
    public async Task CreateEvent_AsOrganiser_Returns201()
    {
        var client = NewCookieClient();
        var email = $"org_{Guid.NewGuid():N}@raceday.co.za";

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "O",
            lastName = "User",
            email,
            password = "Pass123!",
            role = "Organiser"
        });
        await client.PostAsJsonAsync("/api/auth/login", new { email, password = "Pass123!" });

        var res = await client.PostAsJsonAsync("/api/events", NewEventBody());

        res.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
