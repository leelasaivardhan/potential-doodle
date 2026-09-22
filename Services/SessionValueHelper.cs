using System.Text;
using Microsoft.AspNetCore.Http;

namespace CandidateRegistrationPortal.Services;

/// <summary>
/// Session helpers implemented using ISession's instance methods only.
/// This avoids relying on SetInt32/GetInt32/SetInt64/GetInt64 extension methods.
/// </summary>
public static class SessionValueHelper
{
    public static void SetInt32(ISession session, string key, int value)
        => session.Set(key, BitConverter.GetBytes(value));

    public static int? GetInt32(ISession session, string key)
    {
        if (!session.TryGetValue(key, out var data) || data is null || data.Length != sizeof(int))
            return null;
        return BitConverter.ToInt32(data, 0);
    }

    public static void SetInt64(ISession session, string key, long value)
        => session.Set(key, BitConverter.GetBytes(value));

    public static long? GetInt64(ISession session, string key)
    {
        if (!session.TryGetValue(key, out var data) || data is null || data.Length != sizeof(long))
            return null;
        return BitConverter.ToInt64(data, 0);
    }

    public static void SetString(ISession session, string key, string value)
        => session.Set(key, Encoding.UTF8.GetBytes(value));

    public static string? GetString(ISession session, string key)
    {
        if (!session.TryGetValue(key, out var data) || data is null)
            return null;
        return Encoding.UTF8.GetString(data);
    }
}
