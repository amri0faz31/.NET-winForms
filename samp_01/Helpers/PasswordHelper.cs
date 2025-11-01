using System;
using System.Security.Cryptography;

namespace samp_01.Helpers
{
    public static class PasswordHelper
    {
        private const int Iterations = 200_000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static (string Hash, string Salt) HashPassword(string password)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
            using var derive = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
            var hash = derive.GetBytes(HashSize);
            return (Convert.ToBase64String(hash), Convert.ToBase64String(saltBytes));
        }

        public static bool VerifyPassword(string password, string base64Salt, string base64Hash)
        {
            var salt = Convert.FromBase64String(base64Salt);
            using var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = derive.GetBytes(HashSize);
            var expected = Convert.FromBase64String(base64Hash);
            return CryptographicOperations.FixedTimeEquals(hash, expected);
        }
    }
}