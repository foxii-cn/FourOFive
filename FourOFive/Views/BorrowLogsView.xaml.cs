using FourOFive.ViewModels;
using FourOFive.Views.Windows;
using ReactiveUI;

namespace FourOFive.Views
{
    public partial class BorrowLogsView : ReactiveUserControl<BorrowLogsViewModel>, IChildrenView<MainWindow>
    {
        public BorrowLogsView(BorrowLogsViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            this.WhenActivated(disposableRegistration =>
            {

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
