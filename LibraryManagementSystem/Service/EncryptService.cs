using LibraryManagementSystem.Model;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Serilog.Core;
using System;
using System.Security.Cryptography;

namespace LibraryManagementSystem.Service
{
    public class EncryptService
    {
        // 配置对象
        private readonly Config config;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;

        private KeyDerivationPrf PBKDF2Prf
        {
            get
            {
                return (config.PBKDF2PrfString.ToUpper()) switch
                {
                    "HMACSHA1" => KeyDerivationPrf.HMACSHA1,
                    "HMACSHA256" => KeyDerivationPrf.HMACSHA256,
                    "HMACSHA512" => KeyDerivationPrf.HMACSHA512,
                    _ => KeyDerivationPrf.HMACSHA256
                };
            }
        }

        public EncryptService(Config config, Logger logger)
        {
            LogName = GetType().Name;
            this.config = config;
            this.logger = logger;
        }
        public byte[] CreateNewSalt()
        {
            byte[] salt = null;
            try
            {
                salt = new byte[config.SaltSize];
                using RandomNumberGenerator rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 生成盐出错",
                                    LogName);
            }
            return salt;
        }
        public string HashEncrypt(string password, byte[] salt)
        {
            string hashed = null;
            try
            {
                hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: PBKDF2Prf,
            iterationCount: config.PBKDF2IterationTimes,
            numBytesRequested: config.PBKDF2SizeRequested));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 加密出错",
                                    LogName);
            }
            return hashed;
        }

    }
}
