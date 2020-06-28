using FourOFive.Managers;
using FourOFive.Managers.Implements;
using FourOFive.Services;
using FourOFive.Services.Implements;
using FourOFive.Utilities;
using FourOFive.Utilities.Implements;
using FourOFive.ViewModels;
using FourOFive.ViewModels.Windows;
using FourOFive.Views;
using FourOFive.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FourOFive
{
    public partial class App : Application
    {
        private readonly ServiceProvider serviceProvider;
        public App()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton<IConfigurationManagerFactory, JsonConfigurationManagerFactory>()
                .AddSingleton<IDatabaseModelManager, FreeSQLDatabaseModelManager>()
                .AddSingleton<ILogManagerFactory, SeriLogManagerFactory>()
                .AddSingleton<IBookService, BookService>()
                .AddSingleton<IBorrowService, BorrowService>()
                .AddSingleton<HTTPISBNInfoService>()
                .AddSingleton<IISBNInfoService, CachedHTTPISBNInfoService>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IAuthorityUtility, AuthorityUtility>()
                .AddSingleton<ICreditUtility, CreditUtility>()
                .AddSingleton<IEncryptUtility, EncryptUtility>()
                .AddScoped<MainWindowViewModel>()
                .AddScoped<MainWindow>()
                .AddScoped<LogInViewModel>()
                .AddScoped<LogInView>()
                .AddScoped<UserInfoViewModel>()
                .AddScoped<UserInfoView>()
                .AddScoped<BooksViewModel>()
                .AddScoped<BooksView>()
                .AddScoped<BorrowLogsViewModel>()
                .AddScoped<BorrowLogsView>()
                .AddScoped<RegisterViewModel>()
                .AddScoped<RegisterView>()
                ;

            serviceProvider = serviceCollection.BuildServiceProvider();
        }
        private async Task InitializeAsync(string configPath, Encoding configEncoding)
        {
            SplashScreen splashScreen = new SplashScreen("Resources/Imgs/Backgrounds/Cover.png");
            splashScreen.Show(true);
            await serviceProvider.GetService<IConfigurationManagerFactory>().InitializationAsync(configPath, configEncoding);
            MainWindow mainWindow = serviceProvider.GetService<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.RegisterChildrenView(mainWindow.LoginSideMenu.Name, serviceProvider.GetService<LogInView>());
            mainWindow.RegisterChildrenView(mainWindow.AccountInfoSideMenu.Name, serviceProvider.GetService<UserInfoView>());
            mainWindow.RegisterChildrenView(mainWindow.BooksQuerySideMenu.Name, serviceProvider.GetService<BooksView>());
            mainWindow.RegisterChildrenView(mainWindow.BorrowLogSideMenu.Name, serviceProvider.GetService<BorrowLogsView>());
            mainWindow.RegisterChildrenView(mainWindow.RegisterSideMenu.Name, serviceProvider.GetService<RegisterView>());
            mainWindow.Show();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _ = InitializeAsync("config.json", Encoding.UTF8);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            serviceProvider.DisposeAsync().AsTask().Wait();
            base.OnExit(e);
        }
    }
}
