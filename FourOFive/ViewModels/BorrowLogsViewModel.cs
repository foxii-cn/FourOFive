using DynamicData;
using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using FourOFive.Models.DataPackages;
using FourOFive.Services;
using FourOFive.ViewModels.Windows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FourOFive.ViewModels
{
    public class BorrowLogsViewModel : ReactiveObject, IChildrenViewModel<MainWindowViewModel>, IActivatableViewModel
    {
        private readonly IBorrowService borrowService;
        private readonly ILogManager logger;

        private ObservableAsPropertyHelper<List<BorrowLog>> searchResults;
        public List<BorrowLog> SearchResults => searchResults.Value;

        private ObservableAsPropertyHelper<bool> isSearching;
        public bool IsSearching => isSearching.Value;

        private bool isAll = true;
        public bool IsAll { get => isAll; set => this.RaiseAndSetIfChanged(ref isAll, value); }

        // 用户选中的可归还的搜索结果
        public SourceCache<BorrowLog, Guid> SelectedReturnableSearchResultsSource { get; private set; }

        public ReactiveCommand<Unit, List<BorrowLog>> SearchCommand { get; private set; }
        public ReactiveCommand<Unit, List<(string, string)>> ReturnBookCommand { get; private set; }
        private ObservableAsPropertyHelper<bool> isReturning;
        public bool IsReturning => isReturning.Value;

        public MainWindowViewModel ParentViewModel { get; set; }

        public ViewModelActivator Activator { get; }

        public BorrowLogsViewModel(IBorrowService borrowService, ILogManagerFactory loggerFactory)
        {
            this.borrowService = borrowService;
            logger = loggerFactory.CreateManager<BorrowLogsViewModel>();

            Activator = new ViewModelActivator();
            this.WhenActivated(disposableRegistration =>
            {
                SearchCommand = ReactiveCommand.CreateFromTask(SearchAsync)
                .DisposeWith(disposableRegistration);
                searchResults = SearchCommand.ToProperty(this, vm => vm.SearchResults, scheduler: RxApp.MainThreadScheduler);
                isSearching = SearchCommand.IsExecuting
                .ToProperty(this, vm => vm.IsSearching, scheduler: RxApp.MainThreadScheduler);
                SearchCommand.ThrownExceptions.Subscribe(ex => logger.Error(ex, "查询出错"));

                this.WhenAnyValue(vm => vm.IsAll)
                .Select(a => Unit.Default)
                .InvokeCommand(SearchCommand)
                .DisposeWith(disposableRegistration);

                SelectedReturnableSearchResultsSource = new SourceCache<BorrowLog, Guid>(b => b.Id)
                .DisposeWith(disposableRegistration);

                ReturnBookCommand = ReactiveCommand.CreateFromTask(ReturnBookAsync,
                    SelectedReturnableSearchResultsSource
                    .CountChanged
                    .Select(c => c > 0))
                .DisposeWith(disposableRegistration);
                isReturning = ReturnBookCommand.IsExecuting
                .ToProperty(this, vm => vm.IsReturning, scheduler: RxApp.MainThreadScheduler);
                ReturnBookCommand
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(rs =>
                {
                    SearchCommand.Execute().SubscribeOn(RxApp.MainThreadScheduler).Subscribe();
                    List<string> successfulResults = rs.Where(r => r.Item2 == null).Select(r => $"《{r.Item1}》").ToList();
                    List<string> failedResults = rs.Where(r => r.Item2 != null).Select(r => $"《{r.Item1}》({r.Item2})").ToList();
                    if (successfulResults.Count > 0)
                    {
                        string successful = $"还书成功: {string.Join(", ", successfulResults)}";
                        ParentViewModel.GUINotify.Handle(new GUINotifyingDataPackage
                        {
                            Message = successful,
                            Duration = TimeSpan.FromSeconds(1 + successfulResults.Count),
                            Type = NotifyingType.Success
                        })
                        .Subscribe();
                    }
                    if (failedResults.Count > 0)
                    {
                        string failed = $"还书失败: {string.Join(", ", failedResults)}";
                        ParentViewModel.GUINotify.Handle(new GUINotifyingDataPackage
                        {
                            Message = failed,
                            Duration = TimeSpan.FromSeconds(1 + failedResults.Count),
                            Type = NotifyingType.Error
                        })
                        .Subscribe();
                    }
                });
            });
        }

        public async Task<List<BorrowLog>> SearchAsync()
        {
            bool isAll = IsAll;
            Guid account = ParentViewModel.Account.Id;
            return await Task.Run(async () =>
            {
                if (isAll)
                {
                    return await borrowService.QueryAsync(bl => bl.UserId == account && bl.Book.Id == bl.Book.Id);
                }
                else
                {
                    return await borrowService.QueryOverTimeLeaseAsync(new User { Id = account });
                }
            });
        }
        public async Task<List<(string, string)>> ReturnBookAsync()
        {
            // GUI无法防止异步操作时主线程对选择的查询结果的改动, 因此这边要做缓存
            List<(Guid, string)> toReturnBooks = SelectedReturnableSearchResultsSource.Items.Select(bl => (bl.BookId, bl.Book.Title)).ToList();
            User account = new User { Id = ParentViewModel.Account.Id };
            List<(string, string)> returnResults = new List<(string, string)>(toReturnBooks.Count);
            await Task.Run(async () =>
            {
                foreach ((Guid bookId, string title) in toReturnBooks)
                {
                    try
                    {
                        BorrowLog borrowLog = await borrowService.RevertBookAsync(account, new Book { Id = bookId });
                        returnResults.Add((borrowLog.Book.Title, null));
                    }
                    catch (Exception ex)
                    {
                        returnResults.Add((title, ex.Message));
                    }
                }
            });
            return returnResults;
        }
    }
}
