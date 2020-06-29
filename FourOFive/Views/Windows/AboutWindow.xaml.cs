namespace FourOFive.Views.Windows
{
    public partial class AboutWindow:IChildrenView<MainWindow>
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        public MainWindow ParentView { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
