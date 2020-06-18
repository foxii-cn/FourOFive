using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Models.DataBaseModels;
using LibraryManagementSystem.Models.ValueModels;
using Serilog.Core;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace LibraryManagementSystem.Services
{
    public class ISBNInfoService
    {
        // DAO对象
        private readonly DatabaseModelDAO<ISBNInfo> isbnInfoDAO;
        private readonly ISBNInfoHTTPDAO isbnInfoHTTPDAO;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;


        public ISBNInfoService(DatabaseModelDAO<ISBNInfo> isbnInfoDAO, ISBNInfoHTTPDAO isbnInfoHTTPDAO, Logger logger)
        {
            LogName = GetType().Name;
            this.isbnInfoDAO = isbnInfoDAO;
            this.isbnInfoHTTPDAO = isbnInfoHTTPDAO;
            this.logger = logger;
        }
        public IObservable<ISBNInfoGet> GetISBNInfo(IObservable<ISBNInfoGet> getStream)
        {
            // 先查询一遍数据库
            IObservable<DatabaseModelQuery<ISBNInfo, ISBNInfo>> databaseResult = isbnInfoDAO.Query(getStream
                .Select(getInfo => new DatabaseModelQuery<ISBNInfo, ISBNInfo> { DYWhere = getInfo.ISBN }))
                .Do(queryInfo => { if (queryInfo.Exception != null) logger.Error(queryInfo.Exception, "{LogName}: 在数据库查询{ISBN}时出错", LogName, queryInfo.DYWhere); });
            // 把查到的筛选出来打包成最终结果的一部分
            IObservable<ISBNInfoGet> getDbResult = databaseResult
                .Where(queryInfo => queryInfo.Count == 1)
                .Select(queryInfo => new ISBNInfoGet { ISBN = queryInfo.DYWhere.ToString(), ISBNInfo = queryInfo.DatabaseModels[0] });
            // 把查不到的拿去用API查
            IObservable<ISBNInfoHTTPQuery> httpResult = isbnInfoHTTPDAO.GetISBNInfoByAPI(databaseResult
                .Where(queryInfo => queryInfo.Count == 0)
                .Select(queryInfo => new ISBNInfoHTTPQuery { ISBN = queryInfo.DYWhere.ToString() }))
                .Do(httpQueryInfo => { if (httpQueryInfo.Exception != null) logger.Error(httpQueryInfo.Exception, "{LogName}: 通过网络查询{ISBN}时出错", LogName, httpQueryInfo.ISBN); });
            // 把两次都查不到的筛选出来打包成最终结果的一部分
            IObservable<ISBNInfoGet> getBadResult = httpResult
                .Where(httpQueryInfo => httpQueryInfo.ISBNInfo == null)
                .Select(httpQueryInfo => new ISBNInfoGet { ISBN = httpQueryInfo.ISBN });
            // 把API查到的筛选出来存一遍数据库, 再打包成最终结果的一部分
            IObservable<ISBNInfoGet> getHTTPResult = isbnInfoDAO.Create(httpResult
                .Where(httpQueryInfo => httpQueryInfo.ISBNInfo != null)
                .Select(httpQueryInfo => new DatabaseModelCreate<ISBNInfo> { DatabaseModels = new ISBNInfo[] { httpQueryInfo.ISBNInfo } }))
                .Do(createInfo =>
                {
                    if (createInfo.Exception != null)
                        logger.Error(createInfo.Exception, "{LogName}: 在数据库储存{ISBN}时出错", LogName, createInfo.DatabaseModels[0].ISBN); 
                    else if(createInfo.AffectedRows==0)
                        logger.Error("{LogName}: 在数据库储存{ISBN}时因未知原因失败", LogName, createInfo.DatabaseModels[0].ISBN);
                })
                .Select(createInfo => new ISBNInfoGet { ISBN = createInfo.DatabaseModels[0].ISBN, ISBNInfo = createInfo.DatabaseModels[0] });
            return Observable.Merge(getDbResult, getBadResult, getHTTPResult);  // 合并全部结果
        }
    }
}
