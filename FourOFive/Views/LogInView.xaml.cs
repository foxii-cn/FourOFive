using FourOFive.ViewModels;
using FourOFive.Views.Windows;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FourOFive.Views
{
    public partial class LogInView : ReactiveUserControl<LogInViewModel>, IChildrenView<MainWindow>
    {
        private MainWindow parentView;
        public MainWindow ParentView
        {
            get => parentView; set
            {
                parentView = value;
                ViewModel.ParentViewModel = value.ViewModel;
            }
        }

        public LogInView(LogInViewModel viewModel)
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

                this.BindCommand(ViewModel,
                    vm => vm.LoginCommand,
                    v => v.LoginButton)
                .DisposeWith(disposableRegistration);

                ViewModel.LoggedIn.RegisterHandler(interactioni =>
                {
                    interactioni.SetOutput(Unit.Default);
                    PasswordTextBox.Clear();
                    ParentView.Navigate(ParentView.AccountInfoSideMenu.Name);
                }).DisposeWith(disposableRegistration);
            });
        }

    }
}
