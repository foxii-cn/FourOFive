using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace LibraryManagementSystem.Utilities
{
    public static class JsonUtility
    {
        public static string SerializeObject(object element, bool formatting = false)
        {
            return JsonConvert.SerializeObject(element, formatting ? Formatting.Indented : Formatting.None);
        }
        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
        public static void PopulateObject(string value, object element)
        {
            JsonConvert.PopulateObject(value, element);
        }
    }
    public class JsonTokenReader
    {
        private readonly JObject jObject;
        public JsonTokenReader(string json) => jObject = JObject.Parse(json);
        public object SelectToken(string path) => jObject.SelectToken(path);
        public object[] SelectTokenToArray(string path) => jObject.SelectToken(path)?.ToArray();
    }
}
