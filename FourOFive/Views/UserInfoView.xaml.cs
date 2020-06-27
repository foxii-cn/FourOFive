using FourOFive.ViewModels;
using FourOFive.Views.Windows;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;

namespace FourOFive.Views
{
    public partial class UserInfoView : ReactiveUserControl<UserInfoViewModel>, IChildrenView<MainWindow>
    {
        public UserInfoView(UserInfoViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    vm => vm.UserName,
                    v => v.UserGravatar.Id)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.UserName,
                    v => v.UserNameTextBox.Text)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.Name,
                    v => v.NameTextBox.Text)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.NationalIdentificationNumber,
                    v => v.NationalIdentificationNumberTextBox.Text)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.CreditValue,
                    v => v.CreditValueTextBox.Text)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.AccreditedDays,
                    v => v.AccreditedDaysTextBox.Text)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.AuthorityLevel,
                    v => v.AuthorityLevelTextBox.Text)
                .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    vm => vm.RefreshCommand,
                    v => v.RefreshButton)
                .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                    vm => vm.LogoutCommand,
                    v => v.LogoutButton)
                .DisposeWith(disposableRegistration);

                ViewModel.LoggedOut.RegisterHandler(interactioni =>
                {
                    interactioni.SetOutput(Unit.Default);
                    ParentView.Navigate(ParentView.LoginSideMenu.Name);
                });
            });
        }

        private MainWindow parentView;
        public MainWindow ParentView
        {
            get => parentView; set
            {
                parentView = value;
                ViewModel.ParentViewModel = value.ViewModel;
            }
        }
    }
}
