using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RaceDay.API.Tests;

public class EnrolmentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public EnrolmentTests(CustomWebApplicationFactory factory) => _factory = factory;

    private HttpClient NewCookieClient() =>
        _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            AllowAutoRedirect = false
        });

    [Fact]
    public async Task Participant_CanEnrol_AndEnrolmentIsRecorded()
    {
        // Organiser sets up event & category
        var organiser = NewCookieClient();
        var orgEmail = $"org_{Guid.NewGuid():N}@raceday.co.za";

        await organiser.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "Org",
            lastName = "One",
            email = orgEmail,
            password = "Pass123!",
            role = "Organiser"
        });
        await organiser.PostAsJsonAsync("/api/auth/login", new { email = orgEmail, password = "Pass123!" });

        var eventRes = await organiser.PostAsJsonAsync("/api/events", new
        {
            name = "Soweto Marathon",
            description = "Road race",
            eventDate = DateTime.UtcNow.AddDays(20),
            location = "Soweto",
            distance = 42.2,
            eventType = "Run",
            maxParticipants = 500,
            registrationDeadline = DateTime.UtcNow.AddDays(10),
            status = "Open"
        });
        eventRes.StatusCode.Should().Be(HttpStatusCode.Created);

        var eventJson = await eventRes.Content.ReadFromJsonAsync<dynamic>();
        int eventId = (int)eventJson!.GetProperty("eventID").GetInt32();

        var catRes = await organiser.PostAsJsonAsync("/api/categories", new
        {
            eventID = eventId,
            categoryName = "Senior",
            categoryType = "Age",
            entryFee = 200.00
        });
        catRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var catJson = await catRes.Content.ReadFromJsonAsync<dynamic>();
        int categoryId = (int)catJson!.GetProperty("categoryID").GetInt32();

        // Participant logs in and enrols 
        var participant = NewCookieClient();
        var partEmail = $"part_{Guid.NewGuid():N}@raceday.co.za";

        await participant.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "Lerato",
            lastName = "Nkosi",
            email = partEmail,
            password = "Pass123!",
            role = "Participant"
        });
        await participant.PostAsJsonAsync("/api/auth/login", new { email = partEmail, password = "Pass123!" });

        var enrolRes = await participant.PostAsJsonAsync("/api/enrolments", new
        {
            eventID = eventId,
            categoryID = categoryId
        });

        enrolRes.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify participant sees own enrolment
        var myRes = await participant.GetAsync("/api/enrolments/my");
        myRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await myRes.Content.ReadAsStringAsync();
        list.Should().Contain("Soweto Marathon");
    }

    [Fact]
    public async Task Participant_CannotViewOtherEventEnrolments_Returns403()
    {
        var client = NewCookieClient();
        var email = $"peek_{Guid.NewGuid():N}@raceday.co.za";

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "Peek",
            lastName = "User",
            email,
            password = "Pass123!",
            role = "Participant"
        });
        await client.PostAsJsonAsync("/api/auth/login", new { email, password = "Pass123!" });

        var res = await client.GetAsync("/api/enrolments/event/1");

        res.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthenticated_User_CannotEnrol_Returns403()
    {
        var client = NewCookieClient();
        var res = await client.PostAsJsonAsync("/api/enrolments", new
        {
            eventID = 1,
            categoryID = 1
        });

        res.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}