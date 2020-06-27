using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using FourOFive.Services;
using FourOFive.ViewModels.Windows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

        public ReactiveCommand<Unit, (List<Book>, int, long)> SearchCommand { get; private set; }

        public MainWindowViewModel ParentViewModel { get; set; }

        public ViewModelActivator Activator { get; }

        public BooksViewModel(IBookService bookService, IBorrowService borrowService, ILogManagerFactory loggerFactory)
        {
            this.bookService = bookService;
            this.borrowService = borrowService;
            logger = loggerFactory.CreateManager<BooksViewModel>();

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
                .ToProperty(this, vm => vm.SearchResults);
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
            });
        }
        public async Task<(List<Book>, int, long)> SearchAsync()
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
            int pageAmount = (int)(bookAmount / PageSize);
            if (bookAmount % PageSize > 0)
            {
                pageAmount++;
            }
            return (await bookService.QueryAsync(whereExp, Math.Min(PageIndex, pageAmount), PageSize), pageAmount, bookAmount);
        }
        /*
        public void SearchStarted(FunctionEventArgs<string> info)
        {
            _queryCondition = @"Title LIKE @keyword OR Author LIKE @keyword OR PublishingHouse LIKE @keyword";
            _queryParms = new { keyword = string.Format("%{0}%", info.Info) };
            PageIndex = PageIndex;
        }
        public void BooksSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (object item in e.RemovedItems)
            {
                _selectedBooks.Remove(item as Book);
            }
            foreach (object item in e.AddedItems)
            {
                _selectedBooks.Add(item as Book);
            }
            NotifyOfPropertyChange(() => CanBorrow);
        }   */
    }
}
