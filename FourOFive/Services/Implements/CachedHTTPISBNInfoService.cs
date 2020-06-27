using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using FourOFive.Models.DataPackages;
using FourOFive.Services.Implements;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FourOFive.Services
{
    public class CachedHTTPISBNInfoService : IISBNInfoService
    {
        private readonly ILogManager logger;
        private readonly IDatabaseModelManager database;
        private readonly HTTPISBNInfoService httpISBNInfo;


        public CachedHTTPISBNInfoService(ILogManagerFactory loggerFactory, IDatabaseModelManager database, HTTPISBNInfoService httpISBNInfo)
        {
            logger = loggerFactory.CreateManager<CachedHTTPISBNInfoService>();
            this.database = database;
            this.httpISBNInfo = httpISBNInfo;
        }
        public IObservable<ISBNInfoGetDataPackage> GetISBNInfo(IObservable<ISBNInfoGetDataPackage> getStream)
        {
            // 先查询一遍数据库
            IObservable<ISBNInfoGetDataPackage> databaseResult = getStream
                .Where(isbnGet => !string.IsNullOrWhiteSpace(isbnGet.ISBN))
                .Select(isbnGet =>
                Observable.FromAsync(async () =>
                {
                    try
                    {
                        isbnGet.ISBNInfo = (await database.SelectAsync<ISBNInfo, ISBNInfo>(isbn => isbn.ISBN == isbnGet.ISBN)).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        isbnGet.Exceptions.Add(ex);
                    }
                    return isbnGet;
                })
            ).Merge();
            // 把查到的筛选出来打包成最终结果的一部分
            IObservable<ISBNInfoGetDataPackage> getDbResult = databaseResult
                .Where(isbnGet => isbnGet.ISBNInfo != null);
            // 把查不到的拿去用API查
            IObservable<ISBNInfoGetDataPackage> httpResult = httpISBNInfo.GetISBNInfo(databaseResult
                .Where(isbnGet => isbnGet.ISBNInfo == null));
            // 把两次都查不到的筛选出来打包成最终结果的一部分
            IObservable<ISBNInfoGetDataPackage> getBadResult = httpResult
                .Where(isbnGet => isbnGet.ISBNInfo == null);
            // 把API查到的筛选出来存一遍数据库, 再打包成最终结果的一部分
            IObservable<ISBNInfoGetDataPackage> getHTTPResult = httpResult
                .Where(isbnGet => isbnGet.ISBNInfo != null)
                .Select(isbnGet =>
                Observable.FromAsync(async () =>
                {
                    try
                    {
                        await database.InsertAsync(new ISBNInfo[] { isbnGet.ISBNInfo });
                    }
                    catch (Exception ex)
                    {
                        isbnGet.Exceptions.Add(ex);
                    }
                    return isbnGet;
                })
                ).Merge();
            return Observable.Merge(getDbResult, getBadResult, getHTTPResult);  // 合并全部结果
        }
    }
}
