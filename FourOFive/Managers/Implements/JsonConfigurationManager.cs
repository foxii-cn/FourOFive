using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FourOFive.Managers.Implements
{
    public class JsonConfigurationManager : IConfigurationManager
    {
        private readonly JObject config;
        private readonly JsonSerializer serializer;

        public JsonConfigurationManager(JObject config, JsonSerializer serializer)
        {
            this.config = config;
            this.serializer = serializer;
        }

        public IConfigurationManager CreateSubManager(string prefixKey)
        {
            if (!(config.SelectToken(prefixKey) is JObject subConfig))
            {
                config.Remove(prefixKey);
                config.Add(prefixKey, subConfig = new JObject());
            }
            return new JsonConfigurationManager(subConfig, serializer);
        }

        public T Get<T>(string key, T defaultValue = default)
        {
            JToken target = config.SelectToken(key);
            if (target == null)
            {
                config.Add(key, JToken.FromObject(defaultValue, serializer));
                return defaultValue;
            }
            else
            {
                T value;
                try
                {
                    value = target.ToObject<T>(serializer);
                }
                catch (Exception)
                {
                    config.Remove(key);
                    config.Add(key, JToken.FromObject(defaultValue, serializer));
                    return defaultValue;
                }
                if (value == null)
                {
                    config.Remove(key);
                    config.Add(key, JToken.FromObject(value = defaultValue, serializer));
                }
                return value;
            }
        }

        public void Set<T>(T value, string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                config.Replace(JToken.FromObject(value, serializer));
            }
            else if (config.ContainsKey(key))
            {
                config.SelectToken(key).Replace(JToken.FromObject(value, serializer));
            }
            else
            {
                config.Add(key, JToken.FromObject(value, serializer));
            }
        }
    }
}
