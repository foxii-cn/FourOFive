using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utilities;
using System.Text;

namespace LibraryManagementSystem.DAO
{
    public class ConfigDAO
    {
        // 配置文件位置
        private readonly string path;
        // 文件编码
        private readonly Encoding encoding;


        public ConfigDAO(string path, Encoding encoding)
        {
            this.path = path;
            this.encoding = encoding;
        }
        public Config LoadConfig()
        {
            Config config = new Config();
            JsonUtility.PopulateObject(FileUtility.ReadAllText(path, encoding), config);
            return config;
        }
        public void Save(Config config)
        {
            FileUtility.WriteAllText(path, JsonUtility.SerializeObject(config, formatting: true), encoding);
        }
    }
}
