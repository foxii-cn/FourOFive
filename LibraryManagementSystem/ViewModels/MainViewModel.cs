
using Caliburn.Micro;
using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Events;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Serilog.Core;
using System.ComponentModel.Composition;
using System.Text;

namespace LibraryManagementSystem.ViewModels
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : Conductor<object>, IHandle<AccountModificationEvent>
    {
        private readonly IEventAggregator _events;
        private readonly LoggerDAO _loggerDAO;
        private readonly ConfigDAO _configDAO;
        private readonly Logger _logger;
        private readonly Config _config;
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

        [ImportingConstructor]
        public MainViewModel(IEventAggregator events)
        {
            GrowlToken = GetType().Name;
            _events = events;
            _events.Subscribe(this);

            _loggerDAO = new LoggerDAO(@".\logs\log.txt", 268435456);
            _logger = _loggerDAO.GetLogger();
            _configDAO = new ConfigDAO(@".\config.json", Encoding.UTF8, _logger);
            _config = _configDAO.LoadConfig(out bool cfgSuc);
            if (!cfgSuc)
                _configDAO.Save(_config);
            IFreeSql sql = new SqlDAO(_config, _logger).GetSql();
            BookDAO bookDAO = new BookDAO(sql, _logger);
            BorrowLogDAO borrowLogDAO = new BorrowLogDAO(sql, _logger);
            UserDAO userDAO = new UserDAO(sql, _logger);
            _creditService = new CreditService(_config, _logger);
            _encryptService = new EncryptService(_config, _logger);
            AuthorityService = new AuthorityService(_config, _logger);
            _bookService = new BookService(bookDAO, _logger);
            _borrowService = new BorrowService(bookDAO, userDAO, borrowLogDAO, _creditService, _logger);
            _userService = new UserService(userDAO, _creditService, _encryptService, AuthorityService, _logger);

            _logInViewModel = new LogInViewModel(_events, _userService, GrowlToken);
            _registerViewModel = new RegisterViewModel(_events, _userService, GrowlToken);
            _booksViewModel = new BooksViewModel(_events, _bookService, _borrowService, GrowlToken);
            _toBeReturnedViewModel = new ToBeReturnedViewModel(_events, _borrowService, GrowlToken);
            _borrowLogsViewModel=new BorrowLogsViewModel(_events, _borrowService, GrowlToken);
        }
        public string GrowlToken { get; }
        public string UserName => Account == null ? "Î´µÇÂ¼" : Account.UserName;
        public User Account { get; private set; }
        public AuthorityService AuthorityService { get; }

        public void Handle(AccountModificationEvent message)
        {
            DeactivateItem(_registerViewModel, false);
            DeactivateItem(_logInViewModel, false);
            Account = message.Account;
            NotifyOfPropertyChange(() => UserName);
            NotifyOfPropertyChange(() => Account);
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
    }
}