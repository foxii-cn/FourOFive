using Caliburn.Micro;
using LibraryManagementSystem.ViewModels;
using System.Windows;

namespace LibraryManagementSystem
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }



        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }

    }
}
