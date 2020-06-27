using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FourOFive.Managers.Implements
{
    public class JsonConfigurationManagerFactory : IConfigurationManagerFactory
    {
        private JObject config;
        private readonly JsonSerializer serializer;

        public JsonConfigurationManagerFactory()
        {
            serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter()
                }
            });
        }

        public string ConfigurationPath { get; private set; }

        public Encoding ConfigurationEncoding { get; private set; }

        public async Task LoadFromFileAsync(string path = null, Encoding encoding = null)
        {
            path ??= ConfigurationPath;
            encoding ??= ConfigurationEncoding;
            try
            {
                using StreamReader sreader = new StreamReader(path, encoding);
                using JsonReader jreader = new JsonTextReader(sreader);
                config = await JObject.LoadAsync(jreader);
            }
            catch (Exception)
            {
                config = new JObject();
            }
        }
        public async Task InitializationAsync(string path, Encoding encoding)
        {
            ConfigurationPath = path;
            ConfigurationEncoding = encoding;
            await LoadFromFileAsync();
        }
        public async Task SaveToFileAsync(string path = null, Encoding encoding = null)
        {
            path ??= ConfigurationPath;
            encoding ??= ConfigurationEncoding;
            try
            {
                using StreamWriter swriter = new StreamWriter(path, false, encoding);
                using JsonWriter jwriter = new JsonTextWriter(swriter) { Formatting = Formatting.Indented };
                await config.WriteToAsync(jwriter);
            }
            catch (Exception)
            {
            }
        }
        public async ValueTask DisposeAsync()
        {
            await SaveToFileAsync();
        }
        public IConfigurationManager CreateManager(string prefixKey)
        {
            if (!(config.SelectToken(prefixKey) is JObject subConfig))
            {
                config.Remove(prefixKey);
                config.Add(prefixKey, subConfig = new JObject());
            }
            return new JsonConfigurationManager(subConfig, serializer);
        }
    }
}
