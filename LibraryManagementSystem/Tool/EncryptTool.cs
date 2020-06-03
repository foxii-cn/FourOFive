using LibraryManagementSystem.Config;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace LibraryManagementSystem.Tool
{
    public static class EncryptTool
    {
        public static byte[] NewSalt
        {
            get
            {
                byte[] salt = new byte[Configuration.Instance.SaltSize];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
                return salt;
            }
        }
        public static string HashEncrypt(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: Configuration.Instance.PBKDF2Prf,
            iterationCount: Configuration.Instance.PBKDF2IterationTimes,
            numBytesRequested: Configuration.Instance.PBKDF2SizeRequested));
        }

    }
}
