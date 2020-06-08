using System.Text;

namespace LibraryManagementSystem.Utilities
{
    public static class FileUtility
    {
        public static string ReadAllText(string path, Encoding encoding)
        {
            return System.IO.File.ReadAllText(path, encoding);
        }
        public static void WriteAllText(string path, string text, Encoding encoding)
        {
            System.IO.File.WriteAllText(path, text, encoding);
        }
    }
}
