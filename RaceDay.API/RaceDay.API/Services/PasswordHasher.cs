using System.Security.Cryptography;
using System.Text;

namespace RaceDay.API.Services;

public class PasswordHasher : IPasswordHasher
{
    private const string Salt = "RaceDay2026_Salt_v1";

    public string Hash(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password + Salt);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }

    public bool Verify(string password, string hash) => Hash(password) == hash;
}
