
using Caliburn.Micro;


namespace LibraryManagementSystem.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public void ShowDrawer()
        {
            ActivateItem(new DrawerViewModel());
        }
    }
}