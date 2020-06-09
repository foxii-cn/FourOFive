using Caliburn.Micro;
using HandyControl.Controls;
using LibraryManagementSystem.Events;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels
{
    public class BorrowLogsViewModel : Screen, IHandle<AccountStateChangedEvent>
    {
        private readonly IEventAggregator _events;
        private readonly BorrowService _borrowService;
        private readonly string _growlToken;
        private readonly int _pageSize = 7;

        private User _account;
        private List<BorrowLog> _borrowLogList;
        private long _booksCount;
        private int _maxPageCount = 1;
        private int _pageIndex = 1;
        public List<BorrowLog> BorrowLogList
        {
            set
            {
                _borrowLogList = value;
                NotifyOfPropertyChange(() => BorrowLogList);
            }
            get
            {
                return _borrowLogList;
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
                PageUpdated();
            }
            get
            {
                return _pageIndex;
            }
        }
        public BorrowLogsViewModel(IEventAggregator events, BorrowService borrowService, string growlToken)
        {
            _events = events;
            _events.Subscribe(this);
            _borrowService = borrowService;
            _growlToken = growlToken;
        }
        public void PageUpdated()
        {
            try
            {
                BorrowLogList = _borrowService.BorrowLogQuery(PageIndex, _pageSize, out long count, _account.Id);
                MaxPageCount = (int)(count / _pageSize) + 1;
                _booksCount = count;
                if (count == 0)
                    Growl.Info("无借阅记录", _growlToken);
            }
            catch (Exception ex)
            {
                BorrowLogList = null;
                MaxPageCount = 1;
                _booksCount = 0;
                Growl.Error(ex.Message, _growlToken);
            }
        }
        public void Query()
        {
            PageIndex = 1;
            if (_booksCount != 0)
                Growl.Info(string.Format("查询到{0}条借阅记录！", _booksCount), _growlToken); ;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Query();
        }
        public void Handle(AccountStateChangedEvent message)
        {
            _account = message.Account;
        }
    }
}
