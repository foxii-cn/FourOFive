using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using FourOFive.Services;
using FourOFive.Utilities;
using FourOFive.ViewModels.Windows;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FourOFive.ViewModels
{
    public class UserInfoViewModel : ReactiveObject, IChildrenViewModel<MainWindowViewModel>, IActivatableViewModel
    {
        private readonly IUserService userService;
        private readonly ICreditUtility creditUtility;
        private readonly IAuthorityUtility authorityUtility;
        private readonly ILogManager logger;

        private ObservableAsPropertyHelper<string> userName;
        public string UserName => userName.Value;

        private ObservableAsPropertyHelper<string> name;
        public string Name => name.Value;

        private ObservableAsPropertyHelper<string> nationalIdentificationNumber;
        public string NationalIdentificationNumber => nationalIdentificationNumber.Value;

        private ObservableAsPropertyHelper<int> creditValue;
        public int CreditValue => creditValue.Value;

        private ObservableAsPropertyHelper<int> accreditedDays;
        public int AccreditedDays => accreditedDays.Value;

        private ObservableAsPropertyHelper<string> authorityLevel;
        public string AuthorityLevel => authorityLevel.Value;

        public ReactiveCommand<Unit, User> RefreshCommand { get; private set; }
        public ReactiveCommand<Unit, User> LogoutCommand { get; private set; }
        public Interaction<Unit, Unit> LoggedOut { get; }
        public MainWindowViewModel ParentViewModel { get; set; }

        public ViewModelActivator Activator { get; }

        public UserInfoViewModel(IUserService userService, ICreditUtility creditUtility, IAuthorityUtility authorityUtility, ILogManagerFactory loggerFactory)
        {
            this.userService = userService;
            this.creditUtility = creditUtility;
            this.authorityUtility = authorityUtility;
            logger = loggerFactory.CreateManager<UserInfoViewModel>();

            LoggedOut = new Interaction<Unit, Unit>();
            Activator = new ViewModelActivator();
            this.WhenActivated(disposableRegistration =>
            {
                userName = this.WhenAnyValue<UserInfoViewModel, string, User>(vm => vm.ParentViewModel.Account, account => account == null ? "游小客" : account.UserName)
                .ToProperty(this, vm => vm.UserName)
                .DisposeWith(disposableRegistration);
                name = this.WhenAnyValue<UserInfoViewModel, string, User>(vm => vm.ParentViewModel.Account, account => account == null ? "游客" : account.Name)
                .ToProperty(this, vm => vm.Name)
                .DisposeWith(disposableRegistration);
                nationalIdentificationNumber = this.WhenAnyValue<UserInfoViewModel, string, User>(vm => vm.ParentViewModel.Account, account => account == null ? "---" : account.NationalIdentificationNumber)
                .ToProperty(this, vm => vm.NationalIdentificationNumber)
                .DisposeWith(disposableRegistration);
                creditValue = this.WhenAnyValue<UserInfoViewModel, int, User>(vm => vm.ParentViewModel.Account, account => account == null ? -1 : account.CreditValue)
                .ToProperty(this, vm => vm.CreditValue)
                .DisposeWith(disposableRegistration);
                accreditedDays = this.WhenAnyValue(vm => vm.ParentViewModel.Account, account => this.creditUtility.GetAccreditedDays(account))
                .ToProperty(this, vm => vm.AccreditedDays)
                .DisposeWith(disposableRegistration);
                authorityLevel = this.WhenAnyValue(vm => vm.ParentViewModel.Account, account => this.authorityUtility.GetLevel(account))
                .Select(al =>
                {
                    return al switch
                    {
                        Utilities.AuthorityLevel.Visitor => "游客",
                        Utilities.AuthorityLevel.Member => "会员",
                        Utilities.AuthorityLevel.Administrator => "管理员",
                        _ => "外星人",
                    };
                })
                .ToProperty(this, vm => vm.AuthorityLevel)
                .DisposeWith(disposableRegistration);
                RefreshCommand = ReactiveCommand.CreateFromTask(RefreshAsync, outputScheduler: RxApp.MainThreadScheduler)
                .DisposeWith(disposableRegistration);
                RefreshCommand.BindTo(ParentViewModel, pvm => pvm.Account);
                RefreshCommand.ThrownExceptions.Subscribe(ex => logger.Error(ex, "查询账户信息时出错"));
                LogoutCommand = ReactiveCommand.Create<User>(() => null)
                .DisposeWith(disposableRegistration);
                LogoutCommand.Subscribe(u =>
                {
                    ParentViewModel.Account = u;
                    LoggedOut.Handle(Unit.Default).Subscribe();
                });
                LogoutCommand.ThrownExceptions.Subscribe(ex => logger.Error(ex, "退出账户时出错"));
            });
        }
        public async Task<User> RefreshAsync()
        {
            User account = (await userService.QueryAsync(u => u.Id == ParentViewModel.Account.Id)).FirstOrDefault();
            return account;
        }

    }
}
