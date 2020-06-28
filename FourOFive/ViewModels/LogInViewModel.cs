using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using FourOFive.Models.DataPackages;
using FourOFive.Services;
using FourOFive.ViewModels.Windows;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace FourOFive.ViewModels
{
    public class LogInViewModel : ReactiveObject, IChildrenViewModel<MainWindowViewModel>, IActivatableViewModel
    {
        private readonly IUserService userService;
        private readonly ILogManager logger;

        private string userName;
        public string UserName { get => userName; set => this.RaiseAndSetIfChanged(ref userName, value); }

        private string password;
        public string Password { get => password; set => this.RaiseAndSetIfChanged(ref password, value); }

        private ObservableAsPropertyHelper<bool> isLoginning;
        public bool IsLoginning => isLoginning.Value;

        public ReactiveCommand<Unit, User> LoginCommand { get; private set; }
        public Interaction<Unit, Unit> LoggedIn { get; }
        public MainWindowViewModel ParentViewModel { get; set; }

        public ViewModelActivator Activator { get; }

        public LogInViewModel(IUserService userService, ILogManagerFactory loggerFactory)
        {
            this.userService = userService;
            logger = loggerFactory.CreateManager<LogInViewModel>();

            LoggedIn = new Interaction<Unit, Unit>();
            Activator = new ViewModelActivator();
            this.WhenActivated(disposableRegistration =>
            {
                LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync,
                this.WhenAnyValue(vm => vm.UserName,
                vm => vm.Password,
                (n, p) => !string.IsNullOrWhiteSpace(n) && !string.IsNullOrWhiteSpace(p)),
                RxApp.MainThreadScheduler)
                .DisposeWith(disposableRegistration);
                LoginCommand.Subscribe(u =>
                {
                    ParentViewModel.Account = u;
                    ParentViewModel.GUINotify.Handle(new GUINotifyingDataPackage { Message = $"欢迎回来~{u.UserName}!", Type = NotifyingType.Success }).Subscribe();
                    LoggedIn.Handle(Unit.Default).Subscribe();
                });
                LoginCommand.ThrownExceptions.Subscribe(ex => logger.Error(ex, "登录出错"));
                isLoginning = LoginCommand.IsExecuting.ToProperty(this, vm => vm.IsLoginning);
            });

        }
        public async Task<User> LoginAsync()
        {
            return await userService.LogInAsync(new User { UserName = userName, Password = password });
        }

    }
}
