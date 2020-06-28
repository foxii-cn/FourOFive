using FourOFive.Models.DataBaseModels;
using FourOFive.Models.DataPackages;
using FourOFive.Utilities;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FourOFive.ViewModels.Windows
{
    public class MainWindowViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IAuthorityUtility authorityUtility;

        private User account;
        public User Account { get => account; set => this.RaiseAndSetIfChanged(ref account, value); }

        private ObservableAsPropertyHelper<AuthorityLevel> accountAuthorityLevel;
        public AuthorityLevel AccountAuthorityLevel => accountAuthorityLevel.Value;

        public Interaction<GUINotifyingDataPackage, Unit> GUINotify { get; }
        public ViewModelActivator Activator { get; }

        public MainWindowViewModel(IAuthorityUtility authorityUtility)
        {

            this.authorityUtility = authorityUtility;

            GUINotify = new Interaction<GUINotifyingDataPackage, Unit>();
            Activator = new ViewModelActivator();
            this.WhenActivated(disposableRegistration =>
            {
                accountAuthorityLevel = this.WhenAnyValue(vm => vm.Account)
                                .Select(account => this.authorityUtility.GetLevel(account))
                                .ToProperty(this, vm => vm.AccountAuthorityLevel)
                                .DisposeWith(disposableRegistration);
            });
        }
    }
}
