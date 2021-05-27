using System;
using System.Security.Cryptography;

namespace SocialCode.API.Services.Auth
{
    public static class PasswordUtils
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const string Secret = "$MYHASH$V1$";

        private static string Hash(string passwordToEncrypt, int iterations)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);
            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(passwordToEncrypt, salt, iterations);
            var hash = pbkdf2.GetBytes(HashSize);

            // Combine salt and hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Format hash with extra information
            return $"{Secret}{iterations}${base64Hash}";
        }
        public static bool IsValidPassword(string encryptedPassword, string plainPassword)
        {
            if (!IsHashSupported(encryptedPassword))
            {
                return false;
            }

            var splittedHashString = encryptedPassword.Replace(Secret, "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];

            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);

            // Get salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(plainPassword, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);
            
            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
        private static bool IsHashSupported(string hashString)
        {
            return hashString.Contains(Secret);
        }
        public static string HashPassword(string password)
        {
            return Hash(password, 10000);
        }
    }
}