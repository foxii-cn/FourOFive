using FourOFive.Managers;
using FourOFive.Managers.Implements;
using FourOFive.Models.DataBaseModels;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FourOFiveNUnitTest
{
    public class JsonConfigurationManagerFactoryTest
    {
        private readonly IConfigurationManagerFactory factory = new JsonConfigurationManagerFactory();
        private readonly IConfigurationManager manager;
        [SetUp]
        public async Task SetupAsync()
        {
            await factory.InitializationAsync("config.json", Encoding.UTF8);
        }

        [Test]
        public async Task GetTestAsync()
        {
            System.Console.WriteLine(manager.Get<int>(key: "fuckers"));
            await factory.SaveToFileAsync();
        }
        [Test]
        public async Task SetTestAsync()
        {
            manager.Set(new List<string> { "”‡ø≠", "À’¥Ô", "ppp" });
            await factory.SaveToFileAsync();
        }
        [Test]
        public async Task LogTestAsync()
        {
            ILogManagerFactory lfactory = new SeriLogManagerFactory(factory);
            lfactory.CreateManager<JsonConfigurationManagerFactoryTest>().Debug("{YCK}∆®—€¬©ÀÆ¡À!", "”‡≥Ãø≠");
            await factory.SaveToFileAsync();
        }
        [Test]
        public async Task DatabaseTestAsync()
        {
            ILogManagerFactory lfactory = new SeriLogManagerFactory(factory);
            new FreeSQLDatabaseModelManager(lfactory, factory);
            await factory.SaveToFileAsync();
        }
        [Test]
        public void DatabaseModelToString()
        {
            System.Console.WriteLine(new User { Authority = 20, Name = "fuck" });
        }
    }
}