
using Caliburn.Micro;
using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Events;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Views;
using Serilog.Core;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace LibraryManagementSystem.ViewModels
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : Conductor<object>, IHandle<AccountStateChangedEvent>
    {
        private readonly IEventAggregator _events;
        private readonly LoggerDAO _loggerDAO;
        private readonly Logger _logger;
        private readonly ConfigDAO _configDAO;
        private readonly ConfigService _configService;
        private readonly BookService _bookService;
        private readonly BorrowService _borrowService;
        private readonly CreditService _creditService;
        private readonly EncryptService _encryptService;
        private readonly UserService _userService;
        private readonly LogInViewModel _logInViewModel;
        private readonly RegisterViewModel _registerViewModel;
        private readonly BooksViewModel _booksViewModel;
        private readonly ToBeReturnedViewModel _toBeReturnedViewModel;
        private readonly BorrowLogsViewModel _borrowLogsViewModel;
        private readonly UserInfoViewModel _userInfoViewModel;


        [ImportingConstructor]
        public MainViewModel(IEventAggregator events)
        {
            GrowlToken = GetType().Name;
            _events = events;
            _events.Subscribe(this);

            // 初始化日志对象
            _loggerDAO = new LoggerDAO(@".\logs\log.txt", 268435456);
            _logger = _loggerDAO.GetLogger();
            // 初始化配置服务
            _configDAO = new ConfigDAO(@".\config.json", Encoding.UTF8);
            _configService = new ConfigService(_configDAO, _logger);
            _configService.Initialization();
            // 初始化数据库交互类
            IFreeSql sql = new SqlDAO(_configService.GetConfig(), _logger).GetSql();
            BookDAO bookDAO = new BookDAO(sql);
            BorrowLogDAO borrowLogDAO = new BorrowLogDAO(sql);
            UserDAO userDAO = new UserDAO(sql);
            _creditService = new CreditService(_configService.GetConfig(), _logger);
            _encryptService = new EncryptService(_configService.GetConfig(), _logger);
            AuthorityService = new AuthorityService(_configService.GetConfig(), _logger);
            _bookService = new BookService(bookDAO, _logger);
            _borrowService = new BorrowService(bookDAO, userDAO, borrowLogDAO, _creditService, _logger);
            _userService = new UserService(userDAO, _creditService, _encryptService, AuthorityService, _logger);

            _logInViewModel = new LogInViewModel(_events, _userService, GrowlToken);
            _registerViewModel = new RegisterViewModel(_events, _userService, GrowlToken);
            _booksViewModel = new BooksViewModel(_events, _bookService, _borrowService, GrowlToken);
            _toBeReturnedViewModel = new ToBeReturnedViewModel(_events, _borrowService, GrowlToken);
            _borrowLogsViewModel = new BorrowLogsViewModel(_events, _borrowService, GrowlToken);
            _userInfoViewModel = new UserInfoViewModel(_events, _userService, _creditService, AuthorityService, GrowlToken);
        }
        public string GrowlToken { get; }
        public string UserName => Account == null ? "请登录！" : Account.UserName;
        public User Account { get; private set; }
        public AuthorityService AuthorityService { get; }

        public void Handle(AccountStateChangedEvent message)
        {
            if (message.Account != null)
            {
                DeactivateItem(_registerViewModel, false);
                DeactivateItem(_logInViewModel, false);
            }
            else
                DeactivateItem(_userInfoViewModel, false);
            Account = message.Account;
            NotifyOfPropertyChange(() => UserName);
            NotifyOfPropertyChange(() => Account);
        }
        public void OnClosing()
        {
            _configService.Close();
            _loggerDAO.Close(_logger);
        }
        public void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }
        public void ShowAbout()
        {
            new AboutWindow { Owner = Application.Current.MainWindow, DataContext = this }.ShowDialog();
        }
        public void ShowLogin()
        {
            ActivateItem(_logInViewModel);
        }
        public void ShowRegister()
        {
            ActivateItem(_registerViewModel);
        }
        public void ShowBooks()
        {
            ActivateItem(_booksViewModel);
        }
        public void ShowToBeReturned()
        {
            ActivateItem(_toBeReturnedViewModel);
        }
        public void ShowBorrowLogs()
        {
            ActivateItem(_borrowLogsViewModel);
        }
        public void ShowUserInfo()
        {
            ActivateItem(_userInfoViewModel);
        }
    }
}