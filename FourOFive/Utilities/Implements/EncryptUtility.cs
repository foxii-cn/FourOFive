using FourOFive.Managers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FourOFive.Utilities.Implements
{
    public class EncryptUtility : IEncryptUtility
    {
        private readonly ILogManager logger;
        private readonly IConfigurationManager config;

        private readonly int saltSize = 16;
        private readonly KeyDerivationPrf type = KeyDerivationPrf.HMACSHA256;
        private readonly int iteration = 1000;
        private readonly int resultSize = 32;

        public EncryptUtility(ILogManagerFactory loggerFactory, IConfigurationManagerFactory configFactory)
        {
            logger = loggerFactory.CreateManager<EncryptUtility>();
            config = configFactory.CreateManager("PBKDF2Encrypt");

            saltSize = config.Get("SaltSize", saltSize);
            type = config.Get("PRF", type);
            iteration = config.Get("IterationTimes", iteration);
            resultSize = config.Get("SizeRequested", resultSize);
        }
        public async Task<byte[]> CreateNewSaltAsync()
        {
            byte[] salt = new byte[saltSize];
            await Task.Run(() =>
            {
                using RandomNumberGenerator rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);
            });
            return salt;
        }

        public async Task<string> HashEncryptAsync(string password, byte[] salt)
        {
            return await Task.Run(() => Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, type, iteration, resultSize)));
        }
    }
}
