using FourOFive.Managers;
using FourOFive.Services;
using FourOFive.ViewModels.Windows;
using ReactiveUI;

namespace FourOFive.ViewModels
{
    public class BorrowLogsViewModel : ReactiveObject, IChildrenViewModel<MainWindowViewModel>, IActivatableViewModel
    {
        private readonly IBorrowService borrowService;
        private readonly ILogManager logger;



        public MainWindowViewModel ParentViewModel { get; set; }

        public ViewModelActivator Activator { get; }

        public BorrowLogsViewModel(IBorrowService borrowService, ILogManagerFactory loggerFactory)
        {
            this.borrowService = borrowService;
            logger = loggerFactory.CreateManager<BorrowLogsViewModel>();

            Activator = new ViewModelActivator();
            /*this.WhenActivated(disposableRegistration =>
            {

            });*/
        }


    }
}
