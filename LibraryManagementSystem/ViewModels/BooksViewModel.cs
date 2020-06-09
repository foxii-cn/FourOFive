using Caliburn.Micro;
using HandyControl.Controls;
using HandyControl.Data;
using LibraryManagementSystem.Events;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace LibraryManagementSystem.ViewModels
{
    public class BooksViewModel : Screen, IHandle<AccountModificationEvent>
    {
        private readonly IEventAggregator _events;
        private readonly BookService _bookService;
        private readonly BorrowService _borrowService;
        private readonly string _growlToken;
        private readonly int _pageSize = 7;
        private readonly List<Book> _selectedBooks = new List<Book>();


        private User _account;
        private List<Book> _bookList;
        private int _maxPageCount = 1;
        private int _pageIndex = 1;
        private string _queryCondition;
        private object _queryParms;

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
            }
            get
            {
                return _pageIndex;
            }
        }
        public bool CanBorrow => _selectedBooks.Count > 0 && !_selectedBooks.Any(book => book.Margin < 1);
        public BooksViewModel(IEventAggregator events, BookService bookService, BorrowService borrowService, string growlToken)
        {
            _events = events;
            _events.Subscribe(this);
            _bookService = bookService;
            _borrowService = borrowService;
            _growlToken = growlToken;
        }
        public void PageUpdated()
        {
            try
            {
                BookList = _bookService.BookQuery(_queryCondition, PageIndex, _pageSize, out long count, _queryParms);
                MaxPageCount = (int)(count / _pageSize) + 1;
                if (BookList.Count == 0)
                    Growl.Info("图书库为空", _growlToken);
            }
            catch (Exception ex)
            {
                BookList = null;
                MaxPageCount = 1;
                Growl.Error(ex.Message, _growlToken);
            }
        }
        public void SearchStarted(FunctionEventArgs<string> info)
        {
            _queryCondition = @"Title LIKE @keyword OR Author LIKE @keyword OR PublishingHouse LIKE @keyword";
            _queryParms = new { keyword = String.Format("%{0}%", info.Info) };
            if (PageIndex == 1)
                PageUpdated();
            else
                PageIndex = 1;
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
        public void Borrow()
        {
            foreach (Book book in _selectedBooks)
                try
                {
                    int accreditedDays = _borrowService.BorrowBook(_account.Id, book.Id);
                    Growl.Success(String.Format("成功借阅《{0}》，时长{1}天", book.Title, accreditedDays), _growlToken);
                }
                catch (Exception ex)
                {
                    Growl.Error(String.Format("借阅《{0}》失败：{1}", book.Title, ex.Message), _growlToken);
                }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            PageUpdated();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _selectedBooks.Clear();
        }
        public void Handle(AccountModificationEvent message)
        {
            _account = message.Account;
        }
    }
}
