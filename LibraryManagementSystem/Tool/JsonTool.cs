using LibraryManagementSystem.LogSys;
using Newtonsoft.Json;
using System;

namespace LibraryManagementSystem.Tool
{
    public static class JsonTool
    {
        public static readonly string LogName = "JsonTool";

        public static string SerializeObject(object element, bool formatting = false)
        {
            string json;
            try
            {
                json= JsonConvert.SerializeObject(element, formatting ? Formatting.Indented : Formatting.None);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Error(ex, "{LogName}: 序列化{Element}为Json时出错",
                    LogName, element);
                throw;
            }
            return json;
        }
        public static T DeserializeObject<T>(string value)
        {
            T element;
            try
            {
                element = JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Error(ex, "{LogName}: 解析Json{Value}时出错", 
                    LogName, value);
                throw;
            }
            return element;
        }
        public static void PopulateObject(string value, object element)
        {
            try
            {
                JsonConvert.PopulateObject(value, element);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Error(ex, "{LogName}: 解析Json{Value}到{Element}时出错", 
                    LogName, value, element);
                throw;
            }
        }
    }
}
