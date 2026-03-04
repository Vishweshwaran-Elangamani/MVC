using System;
using System.Security.Cryptography;
using System.Text;

namespace BubbleApp.Core.Security
{
    public static class WorkspaceKeyUtil
    {
        public static string NewKey()
        {
            Span<byte> buf = stackalloc byte[32]; // 256-bit
            RandomNumberGenerator.Fill(buf);
            return ToBase64Url(buf);
        }

        public static string Sha256Base64Url(string key)
        {
            var bytes = Encoding.UTF8.GetBytes(key);
            var hash = SHA256.HashData(bytes);
            return ToBase64Url(hash);
        }

        private static string ToBase64Url(ReadOnlySpan<byte> bytes)
        {
            var b64 = Convert.ToBase64String(bytes);
            return b64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }
    }
}