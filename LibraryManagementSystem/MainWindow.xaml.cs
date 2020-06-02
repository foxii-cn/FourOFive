using LibraryManagementSystem.Config;
using LibraryManagementSystem.Controller;
using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Entity;
using LibraryManagementSystem.LogSys;
using LibraryManagementSystem.Service;
using System.Windows;

namespace LibraryManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Book b;
        readonly Member m;
        public MainWindow()
        {
            InitializeComponent();
            b = new Book { Author = "余程凯", Title = "Fuck", Margin = 20 };
            m = new Member { Name = "于洋", CreditValue = 30, NationalIdentificationNumber = "这里什么都没有哈哈哈神奇吧", UserName = "Lucy", Password = "Lucy123456" };
            DatabaseService<Member>.Create(m);
            DatabaseService<Book>.Create(b);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BorrowController.RevertBook(m, b);
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BorrowController.BorrowBook(m, b);
        }
    }
}
