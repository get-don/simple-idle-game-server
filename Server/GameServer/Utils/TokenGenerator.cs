using System.Security.Cryptography;

namespace GameServer.Utils;

public static class TokenGenerator
{
    // 32 bytes = 256-bit
    public static string CreateSessionToken(int bytes = 32)
    {
        Span<byte> buffer = stackalloc byte[32];
        RandomNumberGenerator.Fill(buffer);

        // Base64Url (패딩 제거, URL-safe)
        var token = Convert.ToBase64String(buffer)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        return token;
    }
}
