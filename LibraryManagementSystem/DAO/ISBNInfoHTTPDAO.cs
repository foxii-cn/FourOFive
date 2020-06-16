using LibraryManagementSystem.Models;
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


        public ISBNInfoHTTPDAO(Config config)
        {
            this.config = config;
        }
        public IObservable<ISBNInfoAPIResult> GetISBNInfoByAPI(IObservable<string> isbnStream)
        {
            return isbnStream
                  .Select(isbn =>
                  {
                      string apiUrl = config.ISBNApiUrl.Replace("{isbn}", isbn);
                      return Observable
                      .Empty<string>(ThreadPoolScheduler.Instance)
                      .Delay(TimeSpan.FromMilliseconds(config.GetInterval))
                      .StartWith(apiUrl);
                  })
                  .Concat()
                  .Select(apiUrl =>
                  Observable.FromAsync(async () =>
                       {
                           ISBNInfo isbnInfo = null;
                           Exception exception = null;
                           try
                           {
                               string bookInfoJSON = await HTTPUtility.ReadAsStringAsync(apiUrl);
                               JsonTokenReader reader = new JsonTokenReader(bookInfoJSON);
                               object[] labels = reader.SelectTokenToArray(config.LabelsJsonPath);
                               isbnInfo = new ISBNInfo
                               {
                                   ISBN = reader.SelectToken(config.ISBNJsonPath)?.ToString(),
                                   Title = reader.SelectToken(config.TitleJsonPath)?.ToString(),
                                   Author = reader.SelectToken(config.AuthorJsonPath)?.ToString(),
                                   PublishingHouse = reader.SelectToken(config.PublishingHouseJsonPath)?.ToString(),
                                   Labels = labels == null ? null : string.Join(",", labels.Where(t => t != null).Select(t => t.ToString())),
                                   CoverUrl = reader.SelectToken(config.CoverUrlJsonPath)?.ToString(),
                                   Abstract = reader.SelectToken(config.AbstractJsonPath)?.ToString()
                               };
                           }
                           catch (Exception ex)
                           {
                               exception = ex;
                           }
                           return new ISBNInfoAPIResult() { ISBNInfo = isbnInfo, Exception = exception };
                       })
                  )
                  .Merge();
        }
    }
}
