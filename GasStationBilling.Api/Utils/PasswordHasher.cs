using System.Security.Cryptography;
using System.Text;

namespace GasStationBilling.Api.Utils;

public static class PasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return $"$PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string stored)
    {
        if (string.IsNullOrWhiteSpace(stored)) return false;

        if (stored.StartsWith("$PBKDF2$"))
        {
            var parts = stored.Split('$');
            if (parts.Length != 5) return false;

            var iterations = int.Parse(parts[2]);
            var salt = Convert.FromBase64String(parts[3]);
            var expected = Convert.FromBase64String(parts[4]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        // Backward compatibility with the old SHA256-without-salt format.
        return HashLegacy(password) == stored;
    }

    public static bool IsLegacyFormat(string stored)
        => !string.IsNullOrWhiteSpace(stored) && !stored.StartsWith("$PBKDF2$");

    private static string HashLegacy(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
