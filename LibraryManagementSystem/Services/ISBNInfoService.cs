using LibraryManagementSystem.DAO;
using Serilog.Core;

namespace LibraryManagementSystem.Services
{
    public class ISBNInfoService
    {
        // DAO对象
        private readonly ISBNInfoDAO isbnInfoDAO;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;


        public ISBNInfoService(ISBNInfoDAO isbnInfoDAO, Logger logger)
        {
            LogName = GetType().Name;
            this.isbnInfoDAO = isbnInfoDAO;
            this.logger = logger;
        }

    }
}
