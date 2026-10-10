namespace RaceDay.API.Services;

public static class RaceDaySession
{
    public const string UserIdKey = "UserId";
    public const string RoleKey = "Role";

    public static int? GetUserId(this ISession session)
        => session.GetInt32(UserIdKey);

    public static string? GetRole(this ISession session)
        => session.GetString(RoleKey);

    public static bool IsOrganiser(this ISession session)
        => session.GetString(RoleKey) == "Organiser";

    public static bool IsParticipant(this ISession session)
        => session.GetString(RoleKey) == "Participant";
}