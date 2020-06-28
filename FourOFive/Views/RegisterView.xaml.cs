using FourOFive.ViewModels;
using FourOFive.Views.Windows;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FourOFive.Views
{
    public partial class RegisterView : ReactiveUserControl<RegisterViewModel>, IChildrenView<MainWindow>
    {
        public RegisterView(RegisterViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;

            this.WhenActivated(disposableRegistration =>
            {
                this.WhenAnyValue(v => v.UserNameTextBox.Text)
                .BindTo(ViewModel, vm => vm.UserName)
                .DisposeWith(disposableRegistration);

                Observable.FromEventPattern(PasswordTextBox, nameof(PasswordTextBox.PasswordChanged))
                .Select(ep => PasswordTextBox.Password)
                .BindTo(ViewModel, vm => vm.Password)
                .DisposeWith(disposableRegistration);

                Observable.FromEventPattern(PasswordRepeatTextBox, nameof(PasswordRepeatTextBox.PasswordChanged))
                .Select(ep => PasswordRepeatTextBox.Password)
                .BindTo(ViewModel, vm => vm.PasswordRepeat)
                .DisposeWith(disposableRegistration);

                this.WhenAnyValue(v => v.NameTextBox.Text)
                .BindTo(ViewModel, vm => vm.Name)
                .DisposeWith(disposableRegistration);

                this.WhenAnyValue(v => v.NationalIdentificationNumberTextBox.Text)
                .BindTo(ViewModel, vm => vm.NationalIdentificationNumber)
                .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    vm => vm.RegisterCommand,
                    v => v.RegisterButton)
                .DisposeWith(disposableRegistration);

                ViewModel.Registered.RegisterHandler(interactioni =>
                {
                    interactioni.SetOutput(Unit.Default);
                    UserNameTextBox.Clear();
                    PasswordTextBox.Clear();
                    PasswordRepeatTextBox.Clear();
                    NameTextBox.Clear();
                    NationalIdentificationNumberTextBox.Clear();
                    ParentView.Navigate(ParentView.AccountInfoSideMenu.Name);
                }).DisposeWith(disposableRegistration);
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
