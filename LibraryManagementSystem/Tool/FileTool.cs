using LibraryManagementSystem.LogSys;
using System;
using System.IO;
using System.Text;

namespace LibraryManagementSystem.Tool
{
    public static class FileTool
    {
        public static readonly string LogName = "FileTool";

        public static string ReadAllText(string path, Encoding encoding)
        {
            string text;
            try
            {
                text = File.ReadAllText(path, encoding);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Error(ex, "{LogName}: 以编码{Encoding}读取文件{Path}时出错",
                    LogName, encoding, path);
                throw;
            }
            return text;
        }
        public static void WriteAllText(string path, string text, Encoding encoding)
        {
            try
            {
                File.WriteAllText(path, text, encoding);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Error(ex, "{LogName}: 以编码{Encoding}写入{Text}到文件{Path}时出错",
                    LogName, encoding, text, path);
                throw;
            }
        }
    }
}
