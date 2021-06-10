using System;
using System.Security.Cryptography;

namespace SocialCode.API.Services.Auth
{
    public static class PasswordUtils
    {
        private const int SALT_SIZE = 16;
        private const int HASH_SIZE = 20;
        private const string SECRET = "$MYHASH$V1$";

        public static string HashPassword(string password)
        {
            return Hash(password, 10000);
        }

        public static bool IsValidPassword(string encryptedPassword, string plainPassword)
        {
            if (!IsHashSupported(encryptedPassword)) return false;

            var splittedHashString = encryptedPassword.Replace(SECRET, "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];

            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);

            // Get salt
            var salt = new byte[SALT_SIZE];
            Array.Copy(hashBytes, 0, salt, 0, SALT_SIZE);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(plainPassword, salt, iterations);
            var hash = pbkdf2.GetBytes(HASH_SIZE);

            for (var i = 0; i < HASH_SIZE; i++)
                if (hashBytes[i + SALT_SIZE] != hash[i])
                    return false;
            return true;
        }

        private static bool IsHashSupported(string hashString)
        {
            return hashString.Contains(SECRET);
        }
        
        private static string Hash(string passwordToEncrypt, int iterations)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SALT_SIZE]);
            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(passwordToEncrypt, salt, iterations);
            var hash = pbkdf2.GetBytes(HASH_SIZE);

            // Combine salt and hash
            var hashBytes = new byte[SALT_SIZE + HASH_SIZE];
            Array.Copy(salt, 0, hashBytes, 0, SALT_SIZE);
            Array.Copy(hash, 0, hashBytes, SALT_SIZE, HASH_SIZE);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Format hash with extra information
            return $"{SECRET}{iterations}${base64Hash}";
        }
    }
}