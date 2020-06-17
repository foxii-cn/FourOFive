using Caliburn.Micro;
using HandyControl.Controls;
using HandyControl.Data;
using LibraryManagementSystem.Events;
using LibraryManagementSystem.Models.DataBaseModels;
using LibraryManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LibraryManagementSystem.ViewModels
{
    public class BooksViewModel : Screen, IHandle<AccountStateChangedEvent>
    {
        private readonly IEventAggregator _events;
        private readonly BookService _bookService;
        private readonly BorrowService _borrowService;
        private readonly string _growlToken;
        private readonly int _pageSize = 7;
        private readonly List<Book> _selectedBooks = new List<Book>();


        private User _account;
        private List<Book> _bookList;
        private long _booksCount;
        private int _maxPageCount = 1;
        private int _pageIndex = 1;
        private string _queryCondition;
        private object _queryParms;
        private bool _isQuerying = false;

        public List<Book> BookList
        {
            set
            {
                _bookList = value;
                NotifyOfPropertyChange(() => BookList);
            }
            get
            {
                return _bookList;
            }
        }
        public int MaxPageCount
        {
            set
            {
                _maxPageCount = value;
                NotifyOfPropertyChange(() => MaxPageCount);
            }
            get
            {
                return _maxPageCount;
            }
        }
        public int PageIndex
        {
            set
            {
                _pageIndex = value;
                NotifyOfPropertyChange(() => PageIndex);
                PageUpdatedAsync();
            }
            get
            {
                return _pageIndex;
            }
        }
        public bool IsQuerying
        {
            set
            {
                _isQuerying = value;
                NotifyOfPropertyChange(() => QueryingStateText);
                NotifyOfPropertyChange(() => QueryingLoadingLineVisibility);
                NotifyOfPropertyChange(() => CanSearch);
                NotifyOfPropertyChange(() => CanBorrow);
            }
            get
            {
                return _isQuerying;
            }
        }
        public string QueryingStateText => IsQuerying ? "正在检索数据库..." : _booksCount > 0 ? $"已为您查找到{_booksCount}本符合条件的书！" : "没有符合条件的图书记录！";
        public Visibility QueryingLoadingLineVisibility => IsQuerying ? Visibility.Visible : Visibility.Collapsed;
        public bool CanSearch => !IsQuerying;
        public bool CanBorrow => _selectedBooks.Count > 0 && !_selectedBooks.Any(book => book.Margin < 1) && !IsQuerying;
        public BooksViewModel(IEventAggregator events, BookService bookService, BorrowService borrowService, string growlToken)
        {
            _events = events;
            _events.Subscribe(this);
            _bookService = bookService;
            _borrowService = borrowService;
            _growlToken = growlToken;
        }
        public async void PageUpdatedAsync()
        {
            IsQuerying = true;
            string threadID = "";
            List<Book> outBookList = null;
            int outMaxPageCount = 0;
            long outCount = 0;
            string errMessage = null;
            await Task.Run(() =>
              {
                  try
                  {
                      threadID = Thread.CurrentThread.ManagedThreadId.ToString();
                      outBookList = _bookService.BookQuery(_queryCondition, PageIndex, _pageSize, out long count, _queryParms);
                      outMaxPageCount = (int)(count / _pageSize);
                      if (count % _pageSize > 0)
                          outMaxPageCount++;
                      outCount = count;
                  }
                  catch (Exception ex)
                  {
                      outBookList = null;
                      outMaxPageCount = 1;
                      outCount = 0;
                      errMessage = ex.Message;
                  }
              });
            if (!string.IsNullOrEmpty(errMessage))
                Growl.Error(errMessage, _growlToken);
            BookList = outBookList;
            MaxPageCount = outMaxPageCount;
            _booksCount = outCount;
            IsQuerying = false;
        }
        public void SearchStarted(FunctionEventArgs<string> info)
        {
            _queryCondition = @"Title LIKE @keyword OR Author LIKE @keyword OR PublishingHouse LIKE @keyword";
            _queryParms = new { keyword = String.Format("%{0}%", info.Info) };
            PageIndex = PageIndex;
        }
        public void BooksSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (Object item in e.RemovedItems)
            {
                _selectedBooks.Remove(item as Book);
            }
            foreach (Object item in e.AddedItems)
            {
                _selectedBooks.Add(item as Book);
            }
            NotifyOfPropertyChange(() => CanBorrow);
        }
        public async Task BorrowAsync()
        {
            List<Book> toBorrow = new List<Book>(_selectedBooks);
            PageIndex = PageIndex;
            List<string> successfulInfos = new List<string>(toBorrow.Count);
            List<string> failedInfos = new List<string>(toBorrow.Count);
            await Task.Run(() =>
            {
                foreach (Book book in toBorrow)
                    try
                    {
                        int accreditedDays = _borrowService.BorrowBook(_account.Id, book.Id);
                        successfulInfos.Add($"《{book.Title}》:{accreditedDays}天");
                    }
                    catch (Exception ex)
                    {
                        failedInfos.Add($"《{book.Title}》：{ex.Message}");
                    }
            });
            if (successfulInfos.Count > 0)
                Growl.Success($"成功借阅:\n{string.Join("\n", successfulInfos)}", _growlToken);
            if (failedInfos.Count > 0)
                Growl.Error($"借阅失败:\n{string.Join("\n", failedInfos)}", _growlToken);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            PageIndex = PageIndex;
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _selectedBooks.Clear();
        }
        public void Handle(AccountStateChangedEvent message)
        {
            _account = message.Account;
        }
    }
}
