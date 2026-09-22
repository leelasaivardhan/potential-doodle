using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace CandidateRegistrationPortal.Services;

public class SecurityService(IDataProtectionProvider provider)
{
    private readonly IDataProtector _protector = provider.CreateProtector("CandidateRegistrationPortal.Mobile.v1");

    public string HashMobile(string mobile)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(mobile)));

    public string ProtectMobile(string mobile)
        => Convert.ToBase64String(_protector.Protect(Encoding.UTF8.GetBytes(mobile)));

    public string UnprotectMobile(string protectedMobile)
        => Encoding.UTF8.GetString(_protector.Unprotect(Convert.FromBase64String(protectedMobile)));

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"PBKDF2$100000${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split('$');
        if (parts.Length != 4 || parts[0] != "PBKDF2") return false;
        if (!int.TryParse(parts[1], out var iterations)) return false;
        var salt = Convert.FromBase64String(parts[2]);
        var expected = Convert.FromBase64String(parts[3]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}

public class CaptchaService
{
    public (string Question, int Answer) Generate()
    {
        var random = RandomNumberGenerator.GetInt32(2, 10);
        var random2 = RandomNumberGenerator.GetInt32(2, 10);
        return ($"{random} + {random2} = ?", random + random2);
    }

    public string GenerateOtp() => RandomNumberGenerator.GetInt32(100000, 1_000_000).ToString();
}
