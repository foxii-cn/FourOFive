using FourOFive.ViewModels;
using FourOFive.Views.Windows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FourOFive.Views
{
    /// <summary>
    /// BooksAdminView.xaml 的交互逻辑
    /// </summary>
    public partial class BooksAdminView : ReactiveUserControl<BooksAdminViewModel>, IChildrenView<MainWindow>
    {
        public BooksAdminView()
        {
            InitializeComponent();
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
