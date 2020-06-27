using System.Threading.Tasks;

namespace FourOFive.Utilities
{
    public interface IEncryptUtility
    {
        public Task<byte[]> CreateNewSaltAsync();
        public Task<string> HashEncryptAsync(string password, byte[] salt);
    }
}
