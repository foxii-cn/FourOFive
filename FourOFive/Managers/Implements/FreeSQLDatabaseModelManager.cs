using FourOFive.Models.DataBaseModels;
using FreeSql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FourOFive.Managers.Implements
{
    public class FreeSQLDatabaseModelManager : IDatabaseModelManager
    {
        private readonly IFreeSql sql;
        private readonly ILogManager logger;
        private readonly IConfigurationManager config;

        private readonly DataType type = DataType.Sqlite;
        private readonly bool autoSyncStructure = false;
        private readonly bool parameterized = true;
        private readonly bool sqlToVerbose = true;
        private readonly bool warnOnOverTime = true;
        private readonly long overTime = 200;

        #region MySql配置
        private readonly string mySqlDataSource = "127.0.0.1";
        private readonly string mySqlPort = "3306";
        private readonly string mySqlUserId = "root";
        private readonly string mySqlPassword = "root";
        private readonly string mySqlInitialCatlog = "four_o_five";
        private readonly string mySqlCharset = "utf8";
        private readonly string mySqlSslMode = "none";
        private readonly string mySqlMinPoolSize = "1";
        #endregion
        #region PostgreSql配置
        private readonly string postgreSqlHost = "127.0.0.1";
        private readonly string postgreSqlPort = "3306";
        private readonly string postgreSqlUserName = "root";
        private readonly string postgreSqlPassword = "root";
        private readonly string postgreSqlDatabase = "four_o_five";
        private readonly string postgreSqlPooling = "true";
        private readonly string postgreSqlMinPoolSize = "1";
        #endregion
        #region SqlServer配置
        private readonly string sqlServerDataSource = ".";
        private readonly string sqlServerIntegratedSecurity = "True";
        private readonly string sqlServerInitialCatlog = "four_o_five";
        private readonly string sqlServerPooling = "true";
        private readonly string sqlServerMinPoolSize = "1";
        #endregion
        #region Oracle配置
        private readonly string oracleDataSource = "//127.0.0.1:1521/four_o_five";
        private readonly string oracleUserId = "root";
        private readonly string oraclePassword = "root";
        private readonly string oraclePooling = "true";
        private readonly string oracleMinPoolSize = "1";
        #endregion
        #region Sqlite配置
        private readonly string sqliteDataSource = "./data.db";
        private readonly string sqliteAttachs = "";
        private readonly string sqlitePooling = "true";
        private readonly string sqliteMinPoolSize = "1";
        #endregion
        #region MsAccess配置
        private readonly string msAccessDataSource = "./data.mdb";
        private readonly string msAccessProvider = "Microsoft.Jet.OleDb.4.0";
        private readonly string msAccessMaxPoolSize = "5";
        #endregion

        private readonly string sqlConnectionString;

        public FreeSQLDatabaseModelManager(ILogManagerFactory loggerFactory, IConfigurationManagerFactory configFactory)
        {
            logger = loggerFactory.CreateManager<FreeSQLDatabaseModelManager>();
            config = configFactory.CreateManager("Database");

            type = config.Get("Type", type);
            autoSyncStructure = config.Get("AutoSyncStructure", autoSyncStructure);
            parameterized = config.Get("ParameterizedQuery", parameterized);
            sqlToVerbose = config.Get("OutPutSqlToVerbose", sqlToVerbose);
            warnOnOverTime = config.Get("WarnOnOverTime", sqlToVerbose);
            overTime = config.Get("OverTimeMilliseconds", overTime);
            switch (type)
            {
                case DataType.MySql:
                    IConfigurationManager mySqlConfig = config.CreateSubManager("MySql");
                    mySqlDataSource = mySqlConfig.Get("DataSource", mySqlDataSource);
                    mySqlPort = mySqlConfig.Get("Port", mySqlPort);
                    mySqlUserId = mySqlConfig.Get("UserId", mySqlUserId);
                    mySqlPassword = mySqlConfig.Get("Password", mySqlPassword);
                    mySqlInitialCatlog = mySqlConfig.Get("InitialCatlog", mySqlInitialCatlog);
                    mySqlCharset = mySqlConfig.Get("Charset", mySqlCharset);
                    mySqlSslMode = mySqlConfig.Get("SslMode", mySqlSslMode);
                    mySqlMinPoolSize = mySqlConfig.Get("MinPoolSize", mySqlMinPoolSize);
                    sqlConnectionString = $"Data Source={mySqlDataSource};Port={mySqlPort};User ID={mySqlUserId};Password={mySqlPassword}; Initial Catalog={mySqlInitialCatlog};Charset={mySqlCharset}; SslMode={mySqlSslMode};Min pool size={mySqlMinPoolSize}";
                    break;
                case DataType.SqlServer:
                    IConfigurationManager sqlServerConfig = config.CreateSubManager("SqlServer");
                    sqlServerDataSource = sqlServerConfig.Get("DataSource", sqlServerDataSource);
                    sqlServerIntegratedSecurity = sqlServerConfig.Get("IntegratedSecurity", sqlServerIntegratedSecurity);
                    sqlServerInitialCatlog = sqlServerConfig.Get("InitialCatlog", sqlServerInitialCatlog);
                    sqlServerPooling = sqlServerConfig.Get("Pooling", sqlServerPooling);
                    sqlServerMinPoolSize = sqlServerConfig.Get("PoolSize", sqlServerMinPoolSize);
                    sqlConnectionString = $"Data Source={sqlServerDataSource};Integrated Security={sqlServerIntegratedSecurity};Initial Catalog={sqlServerInitialCatlog};Pooling={sqlServerPooling};Min Pool Size={sqlServerMinPoolSize}";
                    break;
                case DataType.PostgreSQL:
                    IConfigurationManager postgreSqlConfig = config.CreateSubManager("PostgreSql");
                    postgreSqlHost = postgreSqlConfig.Get("Host", postgreSqlHost);
                    postgreSqlPort = postgreSqlConfig.Get("Port", postgreSqlPort);
                    postgreSqlUserName = postgreSqlConfig.Get("UserName", postgreSqlUserName);
                    postgreSqlPassword = postgreSqlConfig.Get("Password", postgreSqlPassword);
                    postgreSqlDatabase = postgreSqlConfig.Get("Database", postgreSqlDatabase);
                    postgreSqlPooling = postgreSqlConfig.Get("Pooling", postgreSqlPooling);
                    postgreSqlMinPoolSize = postgreSqlConfig.Get("MinPoolSize", postgreSqlMinPoolSize);
                    sqlConnectionString = $"Host={postgreSqlHost};Port={postgreSqlPort};Username={postgreSqlUserName};Password={postgreSqlPassword}; config.Database={postgreSqlDatabase};Pooling={postgreSqlPooling};Minimum Pool Size={postgreSqlMinPoolSize}";
                    break;
                case DataType.Oracle:
                    IConfigurationManager oracleConfig = config.CreateSubManager("Oracle");
                    oracleDataSource = oracleConfig.Get("DataSource", oracleDataSource);
                    oracleUserId = oracleConfig.Get("UserId", oracleUserId);
                    oraclePassword = oracleConfig.Get("Password", oraclePassword);
                    oraclePooling = oracleConfig.Get("Pooling", oraclePooling);
                    oracleMinPoolSize = oracleConfig.Get("MinPoolSize", oracleMinPoolSize);
                    sqlConnectionString = $"user id={oracleUserId};password={oraclePassword}; data source={oracleDataSource};Pooling={oraclePooling};Min Pool Size={oracleMinPoolSize}";
                    break;
                default:
                case DataType.Sqlite:
                    IConfigurationManager sqliteConfig = config.CreateSubManager("Sqlite");
                    sqliteDataSource = sqliteConfig.Get("DataSource", sqliteDataSource);
                    sqliteAttachs = sqliteConfig.Get("Attachs", sqliteAttachs);
                    sqlitePooling = sqliteConfig.Get("Pooling", sqlitePooling);
                    sqliteMinPoolSize = sqliteConfig.Get("MinPoolSize", sqliteMinPoolSize);
                    sqlConnectionString = $"Data Source={sqliteDataSource}; Attachs={sqliteAttachs}; Pooling={sqlitePooling};Min Pool Size={sqliteMinPoolSize}";
                    break;
                case DataType.MsAccess:
                    IConfigurationManager msAccessConfig = config.CreateSubManager("MsAccess");
                    msAccessDataSource = msAccessConfig.Get("DataSource", msAccessDataSource);
                    msAccessProvider = msAccessConfig.Get("Provider", msAccessProvider);
                    msAccessMaxPoolSize = msAccessConfig.Get("MaxPoolSize", msAccessMaxPoolSize);
                    sqlConnectionString = $"Provider={msAccessProvider};Data Source={msAccessDataSource};max pool size={msAccessMaxPoolSize}";
                    break;
            }
            sql = new FreeSqlBuilder()
                .UseConnectionString(type, sqlConnectionString)
                .UseAutoSyncStructure(autoSyncStructure)
                .UseGenerateCommandParameterWithLambda(parameterized)
                .Build();
            if (sqlToVerbose)
            {
                sql.Aop.CurdBefore += (s, e) =>
                {
                    logger.Verbose("执行SQL:\n{SQL}", e.Sql);
                };
            }

            if (warnOnOverTime)
            {
                sql.Aop.CurdAfter += (s, e) =>
                {
                    if (e.ElapsedMilliseconds >= overTime)
                    {
                        logger.Warn("执行SQL超时({Time}ms):\n{SQL}", e.ElapsedMilliseconds, e.Sql);
                    }
                };
            }
        }

        public IDatabaseTransactionManager StartTransaction()
        {
            return new FreeSQLDatabaseTransactionManager(sql.CreateUnitOfWork());
        }

        public async Task<long> CountAsync<T>(Expression<Func<T, bool>> whereExp = null, IEnumerable<T> ids = null, DbTransaction transaction = null) where T : DatabaseModel
        {
            return await sql.Select<T>(ids).Where(whereExp).WithTransaction(transaction).CountAsync();
        }

        public async Task<int> DeleteAsync<T, TNavigate>(Expression<Func<T, bool>> whereExp = null, IEnumerable<T> ids = null, Expression<Func<T, TNavigate>> navigateExp = null, DbTransaction transaction = null)
            where T : DatabaseModel
            where TNavigate : DatabaseModel
        {
            return await sql.Select<T>(ids).Where(whereExp).Include(navigateExp).WithTransaction(transaction).ToDelete().ExecuteAffrowsAsync();
        }

        public async Task ForUpdateAsync<T, TNavigate>(Expression<Func<T, bool>> whereExp = null, IEnumerable<T> ids = null, Expression<Func<T, TNavigate>> navigateExp = null, DbTransaction transaction = null)
            where T : DatabaseModel
            where TNavigate : DatabaseModel
        {
            await sql.Select<T>(ids).Where(whereExp).Include(navigateExp).WithTransaction(transaction).ForUpdate().FirstAsync();
        }

        public async Task<int> InsertAsync<T>(IEnumerable<T> elements = null, DbTransaction transaction = null) where T : DatabaseModel
        {
            return await sql.Insert(elements).WithTransaction(transaction).ExecuteAffrowsAsync();
        }

        public async Task<List<T>> SelectAsync<T, TNavigate>(Expression<Func<T, bool>> whereExp = null, IEnumerable<T> ids = null, Expression<Func<T, TNavigate>> navigateExp = null, int pageIndex = 0, int pageSize = 0, DbTransaction transaction = null)
            where T : DatabaseModel
            where TNavigate : DatabaseModel
        {
            ISelect<T> select = sql.Select<T>(ids).Where(whereExp).Include(navigateExp).WithTransaction(transaction);
            if (pageSize > 0)
            {
                select = select.Page(pageIndex, pageSize);
            }

            return await select.ToListAsync();
        }

        public async Task<int> UpdateAsync<T, TMember>(Expression<Func<T, TMember>> setExp, Expression<Func<T, bool>> whereExp = null, IEnumerable<T> ids = null, DbTransaction transaction = null) where T : DatabaseModel
        {
            return await sql.Update<T>(ids).Where(whereExp).Set(setExp).WithTransaction(transaction).ExecuteAffrowsAsync();
        }
    }
}
