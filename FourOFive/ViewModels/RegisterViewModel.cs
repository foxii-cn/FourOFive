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
    public class RegisterViewModel : ReactiveObject, IChildrenViewModel<MainWindowViewModel>, IActivatableViewModel
    {
        private readonly IUserService userService;
        private readonly ILogManager logger;

        private string userName;
        public string UserName { get => userName; set => this.RaiseAndSetIfChanged(ref userName, value); }

        private string password;
        public string Password { get => password; set => this.RaiseAndSetIfChanged(ref password, value); }

        private string passwordRepeat;
        public string PasswordRepeat { get => passwordRepeat; set => this.RaiseAndSetIfChanged(ref passwordRepeat, value); }

        private string name;
        public string Name { get => name; set => this.RaiseAndSetIfChanged(ref name, value); }

        private string nationalIdentificationNumber;
        public string NationalIdentificationNumber { get => nationalIdentificationNumber; set => this.RaiseAndSetIfChanged(ref nationalIdentificationNumber, value); }

        private ObservableAsPropertyHelper<bool> isRegistering;
        public bool IsRegistering => isRegistering.Value;

        public ReactiveCommand<Unit, User> RegisterCommand { get; private set; }
        public Interaction<Unit, Unit> Registered { get; }

        public MainWindowViewModel ParentViewModel { get; set; }

        public ViewModelActivator Activator { get; }

        public RegisterViewModel(IUserService userService, ILogManagerFactory loggerFactory)
        {
            this.userService = userService;
            logger = loggerFactory.CreateManager<LogInViewModel>();

            Registered = new Interaction<Unit, Unit>();
            Activator = new ViewModelActivator();
            this.WhenActivated(disposableRegistration =>
            {
                RegisterCommand = ReactiveCommand.CreateFromTask(RegisterAsync,
                    this.WhenAnyValue(vm => vm.UserName,
                    vm => vm.Password,
                    vm => vm.PasswordRepeat,
                    vm => vm.Name,
                    vm => vm.NationalIdentificationNumber,
                    (un, p, pr, n, nn) =>
                    !string.IsNullOrWhiteSpace(un) &&
                    !string.IsNullOrWhiteSpace(p) &&
                    !string.IsNullOrWhiteSpace(pr) &&
                    !string.IsNullOrWhiteSpace(n) &&
                    !string.IsNullOrWhiteSpace(nn)))
                .DisposeWith(disposableRegistration);
                isRegistering = RegisterCommand.IsExecuting
                .ToProperty(this, vm => vm.IsRegistering);
                RegisterCommand.Subscribe(u =>
                {
                    ParentViewModel.Account = u;
                    ParentViewModel.GUINotify.Handle(new GUINotifyingDataPackage { Message = $"注册成功~{u.UserName}!", Type = NotifyingType.Success }).Subscribe();
                    Registered.Handle(Unit.Default).Subscribe();
                });
                RegisterCommand.ThrownExceptions.Subscribe(ex =>
                {
                    logger.Error(ex, "注册出错");
                    ParentViewModel.GUINotify.Handle(new GUINotifyingDataPackage { Message = $"注册出错: {ex.Message}!", Type = NotifyingType.Error }).Subscribe();
                });
            });
        }


        public async Task<User> RegisterAsync()
        {
            if (!Password.Equals(PasswordRepeat))
            {
                throw new Exception("两次密码不一致! ");
            }

            return await Task.Run(async () => await userService.RegisterAsync(new User
            {
                UserName = UserName,
                Password = Password,
                Name = Name,
                NationalIdentificationNumber = NationalIdentificationNumber
            }));
        }
    }
}
