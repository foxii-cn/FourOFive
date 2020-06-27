using System;
using System.Text;
using System.Threading.Tasks;

namespace FourOFive.Managers
{
    public interface IConfigurationManagerFactory : IAsyncDisposable
    {
        public string ConfigurationPath { get; }
        public Encoding ConfigurationEncoding { get; }
        public Task LoadFromFileAsync(string path = null, Encoding encoding = null);
        public Task SaveToFileAsync(string path = null, Encoding encoding = null);
        public Task InitializationAsync(string path, Encoding encoding);
        public IConfigurationManager CreateManager(string prefixKey);
    }
}
