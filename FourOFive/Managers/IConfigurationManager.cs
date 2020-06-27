namespace FourOFive.Managers
{
    public interface IConfigurationManager
    {
        public T Get<T>(string key, T defaultValue = default);
        public void Set<T>(T value, string key = null);
        public IConfigurationManager CreateSubManager(string prefixKey);
    }
}
