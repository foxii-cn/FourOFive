using System.Net.Http;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Utilities
{
    public static class HTTPUtility
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<string> ReadAsStringAsync(string requestUri)
        {
            return await client.GetStringAsync(requestUri);
        }
    }
}
