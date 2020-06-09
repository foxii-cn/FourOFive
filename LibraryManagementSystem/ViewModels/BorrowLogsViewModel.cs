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
                if (BorrowLogList.Count == 0)
                    Growl.Info("无借阅记录", _growlToken);
            }
            catch (Exception ex)
            {
                BorrowLogList = null;
                MaxPageCount = 1;
                Growl.Error(ex.Message, _growlToken);
            }
        }
        public void Query()
        {
            PageUpdated();
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
