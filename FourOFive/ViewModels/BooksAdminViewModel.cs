using FourOFive.ViewModels.Windows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FourOFive.ViewModels
{
    public class BooksAdminViewModel : ReactiveObject, IChildrenViewModel<MainWindowViewModel>, IActivatableViewModel
    {
        public MainWindowViewModel ParentViewModel { get; set; }

        public ViewModelActivator Activator { get; }


    }
}
