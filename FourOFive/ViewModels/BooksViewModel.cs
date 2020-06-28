using DynamicData;
using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using FourOFive.Models.DataPackages;
using FourOFive.Services;
using FourOFive.ViewModels.Windows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace FourOFive.ViewModels
{
    public class BooksViewModel : ReactiveObject, IChildrenViewModel<MainWindowViewModel>, IActivatableViewModel
    {
        private readonly IBookService bookService;
        private readonly IBorrowService borrowService;
        private readonly ILogManager logger;

        private string searchTerms;
        public string SearchTerms { get => searchTerms; set => this.RaiseAndSetIfChanged(ref searchTerms, value); }

        private int pageIndex = 1;
        public int PageIndex { get => pageIndex; set => this.RaiseAndSetIfChanged(ref pageIndex, value); }

        private int pageSize = 7;
        public int PageSize { get => pageSize; set => this.RaiseAndSetIfChanged(ref pageSize, value); }

        private int pageAmount = 1;
        public int PageAmount { get => pageAmount; set => this.RaiseAndSetIfChanged(ref pageAmount, value); }

        private int bookAmount;
        public int BookAmount { get => bookAmount; set => this.RaiseAndSetIfChanged(ref bookAmount, value); }

        private ObservableAsPropertyHelper<List<Book>> searchResults;
        public List<Book> SearchResults => searchResults.Value;

        private ObservableAsPropertyHelper<bool> isSearching;
        public bool IsSearching => isSearching.Value;

        // 用户选中的搜索结果
        public SourceCache<Book, Guid> SelectedSearchResultsSource { get; private set; }
        public SourceCache<ChosenBookDataPackage, Guid> SelectedToBorrowBooksSource { get; private set; }

        // 待借书栏, 注意这个元素会被异步线程操作
        private SourceCache<ChosenBookDataPackage, Guid> ToBorrowBooksSource { get; }
        private ReadOnlyObservableCollection<ChosenBookDataPackage> toBorrowBooks;
        public ReadOnlyObservableCollection<ChosenBookDataPackage> ToBorrowBooks => toBorrowBooks;

        public ReactiveCommand<Unit, (List<Book>, int, long)> SearchCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> AddToBorrowCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DeleteFromBorrowCommand { get; private set; }

        public ReactiveCommand<Unit, List<(string, int, string)>> BorrowCommand { get; private set; }
        private ObservableAsPropertyHelper<bool> isBorrowing;
        public bool IsBorrowing => isBorrowing.Value;

        public MainWindowViewModel ParentViewModel { get; set; }

        public ViewModelActivator Activator { get; }

        public BooksViewModel(IBookService bookService, IBorrowService borrowService, ILogManagerFactory loggerFactory)
        {
            this.bookService = bookService;
            this.borrowService = borrowService;
            logger = loggerFactory.CreateManager<BooksViewModel>();

            ToBorrowBooksSource = new SourceCache<ChosenBookDataPackage, Guid>(b => b.Id);
            Activator = new ViewModelActivator();
            this.WhenActivated(disposableRegistration =>
            {
                SearchCommand = ReactiveCommand.CreateFromTask(SearchAsync)
                .DisposeWith(disposableRegistration);
                IConnectableObservable<(List<Book>, int, long)> Publisher = SearchCommand.Publish();
                Publisher.Select(t => t.Item3)
                .BindTo(this, vm => vm.BookAmount);
                Publisher.Select(t => t.Item2)
                .BindTo(this, vm => vm.PageAmount);
                searchResults = Publisher.Select(t => t.Item1)
                .ToProperty(this, vm => vm.SearchResults,scheduler:RxApp.MainThreadScheduler);
                Publisher.Connect();
                SearchCommand.ThrownExceptions.Subscribe(ex => logger.Error(ex, "查询出错"));
                isSearching = SearchCommand.IsExecuting
                .ToProperty(this, vm => vm.IsSearching);

                this.WhenAnyValue(vm => vm.SearchTerms)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged()
                .Select(st => Unit.Default)
                .Merge(this.WhenAnyValue(vm => vm.PageIndex, vm => vm.PageSize, (pi, ps) => Unit.Default))
                .InvokeCommand(SearchCommand)
                .DisposeWith(disposableRegistration);

                SelectedSearchResultsSource = new SourceCache<Book, Guid>(b => b.Id)
                .DisposeWith(disposableRegistration);
                SelectedToBorrowBooksSource = new SourceCache<ChosenBookDataPackage, Guid>(b => b.Id)
                .DisposeWith(disposableRegistration);

                ToBorrowBooksSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)  // 主线程订阅
                .Bind(out toBorrowBooks)
                .Subscribe()
                .DisposeWith(disposableRegistration);

                BorrowCommand = ReactiveCommand.CreateFromTask(BorrowBookAsync, 
                    ToBorrowBooksSource.CountChanged  // 待借书栏书本数量大于0时才可执行
                    .Select(c => c > 0)
                    .ObserveOn(RxApp.MainThreadScheduler)) // 主线程订阅
                .DisposeWith(disposableRegistration);
                isBorrowing = BorrowCommand.IsExecuting
                .ToProperty(this, vm => vm.IsBorrowing);
                BorrowCommand
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(rs =>
                {
                    ToBorrowBooksSource.Clear();  //清空待借书栏
                    List<string> successfulResults = rs.Where(r => r.Item2 >= 0).Select(r => $"《{r.Item1}》({r.Item2}天)").ToList();
                    List<string> failedResults = rs.Where(r => r.Item2 < 0).Select(r => $"《{r.Item1}》({r.Item3})").ToList();
                    if (successfulResults.Count > 0)
                    {
                        string successful = $"借书成功: {string.Join(", ", successfulResults)}";
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
                        string failed = $"借书失败: {string.Join(", ", failedResults)}";
                        ParentViewModel.GUINotify.Handle(new GUINotifyingDataPackage
                        {
                            Message = failed,
                            Duration = TimeSpan.FromSeconds(1 + failedResults.Count),
                            Type = NotifyingType.Error
                        })
                        .Subscribe();
                    }
                });

                AddToBorrowCommand = ReactiveCommand.Create(
                    () => ToBorrowBooksSource.AddOrUpdate(SelectedSearchResultsSource.Items.Select(b => new ChosenBookDataPackage(b))),
                    SelectedSearchResultsSource.CountChanged.Select(c => c > 0).CombineLatest(this.WhenAnyValue(vm => vm.IsBorrowing, b => !b), (c, b) => c && b))
                .DisposeWith(disposableRegistration);

                DeleteFromBorrowCommand = ReactiveCommand.Create(
                    () => ToBorrowBooksSource.Remove(SelectedToBorrowBooksSource.Items),
                    SelectedToBorrowBooksSource.CountChanged.Select(c => c > 0).CombineLatest(this.WhenAnyValue(vm => vm.IsBorrowing, b => !b), (c, b) => c && b))
                .DisposeWith(disposableRegistration);
            });
        }
        public async Task<(List<Book>, int, long)> SearchAsync()
        {
            int pageSize = PageSize;
            int pageIndex = PageIndex;
            return await Task.Run(async () =>
            {
                Expression<Func<Book, bool>> whereExp;
                if (SearchTerms != null)
                {
                    whereExp = b => b.Author.Contains(SearchTerms) || b.PublishingHouse.Contains(SearchTerms) || b.Title.Contains(SearchTerms);
                }
                else
                {
                    whereExp = null;
                }
                long bookAmount = await bookService.CountAsync(whereExp);
                int pageAmount = (int)(bookAmount / pageSize);
                if (bookAmount % pageSize > 0)
                {
                    pageAmount++;
                }
                return (await bookService.QueryAsync(whereExp, Math.Min(pageIndex, pageAmount), pageSize), pageAmount, bookAmount);
            });
        }

        public async Task<List<(string, int, string)>> BorrowBookAsync()
        {
            Guid account = ParentViewModel.Account.Id;
            List<(string, int, string)> borrowResults = new List<(string, int, string)>(ToBorrowBooks.Count);
            DateTime now = DateTime.Now;
            await Task.Run(async () =>
            {
                foreach (ChosenBookDataPackage cbdp in ToBorrowBooks)
                {
                    try
                    {
                        BorrowLog leaseLog = await borrowService.BorrowBookAsync(new User { Id = account }, new Book { Id = cbdp.Id });
                        borrowResults.Add((cbdp.Title, (int)(leaseLog.Deadline - now).TotalDays, null));
                    }
                    catch (Exception ex)
                    {
                        borrowResults.Add((cbdp.Title, -1, ex.Message));
                    }
                }
            });
            return borrowResults;
        }
    }
}
