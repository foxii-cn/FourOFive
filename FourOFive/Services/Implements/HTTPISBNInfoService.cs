using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using FourOFive.Models.DataPackages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;

namespace FourOFive.Services.Implements
{
    public class HTTPISBNInfoService : IISBNInfoService, IDisposable
    {
        private readonly HttpClient client;
        private readonly ILogManager logger;
        private readonly IConfigurationManager config;

        private readonly string url = "https://book.feelyou.top/isbn/{isbn}";
        private readonly int interval = 200;
        private readonly string isbnJsonPath = "isbn";
        private readonly string titleJsonPath = "title";
        private readonly string authorJsonPath = "book_info.作者";
        private readonly string publishingHouseJsonPath = "book_info.出版社";
        private readonly string labelsJsonPath = "labels";
        private readonly string coverUrlJsonPath = "cover_url";
        private readonly string abstractJsonPath = "abstract";

        public HTTPISBNInfoService(ILogManagerFactory loggerFactory, IConfigurationManagerFactory configFactory)
        {
            client = new HttpClient();
            logger = loggerFactory.CreateManager<HTTPISBNInfoService>();
            config = configFactory.CreateManager("ISBNAPI");

            url = config.Get("", url);
            interval = config.Get("", interval);
            IConfigurationManager jsonPathConfig = config.CreateSubManager("JsonPath");
            isbnJsonPath = jsonPathConfig.Get("ISBN", isbnJsonPath);
            titleJsonPath = jsonPathConfig.Get("Title", titleJsonPath);
            authorJsonPath = jsonPathConfig.Get("Author", authorJsonPath);
            publishingHouseJsonPath = jsonPathConfig.Get("PublishingHouse", publishingHouseJsonPath);
            labelsJsonPath = jsonPathConfig.Get("Labels", labelsJsonPath);
            coverUrlJsonPath = jsonPathConfig.Get("CoverUrl", coverUrlJsonPath);
            abstractJsonPath = jsonPathConfig.Get("Abstract", abstractJsonPath);
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public IObservable<ISBNInfoGetDataPackage> GetISBNInfo(IObservable<ISBNInfoGetDataPackage> isbnStream)
        {
            return isbnStream
.Where(isbnGet => !string.IsNullOrWhiteSpace(isbnGet.ISBN))
.Select(isbnGet =>
{
    return Observable
    .Empty<ISBNInfoGetDataPackage>()
    .Delay(TimeSpan.FromMilliseconds(interval))
    .StartWith(isbnGet);
})
.Concat()
.Select(isbnGet =>
Observable.FromAsync(async () =>
{
    try
    {
        using StreamReader sreader = new StreamReader(await client.GetStreamAsync(url.Replace("{isbn}", isbnGet.ISBN)));
        using JsonReader jreader = new JsonTextReader(sreader);
        JToken reader = await JToken.LoadAsync(jreader);
        string isbnGot = reader.SelectToken(isbnJsonPath)?.Value<string>();
        if (isbnGet.ISBN.Equals(isbnGot))
        {
            string[] labels = reader.SelectToken(labelsJsonPath)?.Value<string[]>();
            isbnGet.ISBNInfo = new ISBNInfo
            {
                ISBN = isbnGot,
                Title = reader.SelectToken(titleJsonPath)?.Value<string>(),
                Author = reader.SelectToken(authorJsonPath)?.Value<string>(),
                PublishingHouse = reader.SelectToken(publishingHouseJsonPath)?.Value<string>(),
                Labels = labels == null ? null : string.Join(",", labels.Where(t => t != null)),
                CoverUrl = reader.SelectToken(coverUrlJsonPath)?.Value<string>(),
                Abstract = reader.SelectToken(abstractJsonPath)?.Value<string>()
            };
        }
    }
    catch (Exception ex)
    {
        isbnGet.Exceptions.Add(ex);
    }
    return isbnGet;
}))
.Merge();
        }
    }
}
