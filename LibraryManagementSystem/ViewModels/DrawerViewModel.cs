using Caliburn.Micro;

namespace LibraryManagementSystem.ViewModels
{
    public class DrawerViewModel : Screen
    {
        private bool drawerLeft;
        public bool IsOpen
        {
            get { return drawerLeft; }
            set
            {
                drawerLeft = value;
                NotifyOfPropertyChange(() => IsOpen);
            }
        }
    }
}
