using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.DataBaseModels;
using LibraryManagementSystem.Models.ValueModels;
using LibraryManagementSystem.Utilities;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace LibraryManagementSystem.DAO
{
    public class ISBNInfoHTTPDAO
    {
        // 配置对象
        private readonly Config config;


        public ISBNInfoHTTPDAO(Config config) => this.config = config;
        public IObservable<ISBNInfoHTTPQuery> GetISBNInfoByAPI(IObservable<ISBNInfoHTTPQuery> isbnStream) => isbnStream
            .Where(isbnQuery => !string.IsNullOrEmpty(isbnQuery.ISBN))
            .Select(isbnQuery =>
            {
                string apiUrl = config.ISBNApiUrl.Replace("{isbn}", isbnQuery.ISBN);
                return Observable
                .Empty(ThreadPoolScheduler.Instance, new { ISBN = "", ApiUrl = "" })
                .Delay(TimeSpan.FromMilliseconds(config.GetInterval))
                .StartWith(new { isbnQuery.ISBN, ApiUrl = apiUrl });
            })
            .Concat()
            .Select(apiUrl =>
            Observable.FromAsync(async () =>
            {
                ISBNInfo isbnInfo = null;
                Exception exception = null;
                try
                {
                    string bookInfoJSON = await HTTPUtility.ReadAsStringAsync(apiUrl.ApiUrl);

                    JsonTokenReader reader = new JsonTokenReader(bookInfoJSON);
                    string isbnGot = reader.SelectToken(config.ISBNJsonPath)?.ToString();
                    if (apiUrl.ISBN.Equals(isbnGot))
                    {
                        object[] labels = reader.SelectTokenToArray(config.LabelsJsonPath);
                        isbnInfo = new ISBNInfo
                        {
                            ISBN = isbnGot,
                            Title = reader.SelectToken(config.TitleJsonPath)?.ToString(),
                            Author = reader.SelectToken(config.AuthorJsonPath)?.ToString(),
                            PublishingHouse = reader.SelectToken(config.PublishingHouseJsonPath)?.ToString(),
                            Labels = labels == null ? null : string.Join(",", labels.Where(t => t != null).Select(t => t.ToString())),
                            CoverUrl = reader.SelectToken(config.CoverUrlJsonPath)?.ToString(),
                            Abstract = reader.SelectToken(config.AbstractJsonPath)?.ToString()
                        };
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                return new ISBNInfoHTTPQuery() { ISBN = apiUrl.ISBN, ISBNInfo = isbnInfo, Exception = exception };
            })
            )
            .Merge();
    }
}
