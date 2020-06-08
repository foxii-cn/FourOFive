using Caliburn.Micro;
using HandyControl.Controls;
using LibraryManagementSystem.Events;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagementSystem.ViewModels
{
    public class ToBeReturnedViewModel : Screen, IHandle<AccountModificationEvent>
    {
        private readonly IEventAggregator _events;
        private readonly BorrowService _borrowService;
        private readonly string _growlToken;
        private readonly List<BorrowLog> _selectedBorrowLogs = new List<BorrowLog>();

        private User _account;
        private List<BorrowLog> _borrowLogList;
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
        public bool CanRevert => _selectedBorrowLogs.Count > 0;
        public ToBeReturnedViewModel(IEventAggregator events, BorrowService borrowService, string growlToken)
        {
            _events = events;
            _events.Subscribe(this);
            _borrowService = borrowService;
            _growlToken = growlToken;
        }
        public void Query()
        {
            try
            {
                BorrowLogList = _borrowService.TardyLease(_account.Id);
            }
            catch (Exception ex)
            {
                BorrowLogList = null;
                Growl.Error(ex.Message, _growlToken);
            }
            if (BorrowLogList.Count == 0)
                Growl.Info("无待还的书本记录", _growlToken);
        }
        public void BorrowLogsSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (Object item in e.RemovedItems)
            {
                _selectedBorrowLogs.Remove(item as BorrowLog);
            }
            foreach (Object item in e.AddedItems)
            {
                _selectedBorrowLogs.Add(item as BorrowLog);
            }
            NotifyOfPropertyChange(() => CanRevert);
        }
        public void Revert()
        {
            foreach (BorrowLog borrowLog in _selectedBorrowLogs)
                try
                {
                    int creditChange = _borrowService.RevertBook(_account.Id, borrowLog.BookId);
                    Growl.Success(String.Format("成功归还《{0}》，信誉{1}{2}点", borrowLog.Book.Title,
                        creditChange>=0?"增加" :"减少",creditChange), _growlToken);
                }
                catch (Exception ex)
                {
                    Growl.Error(String.Format("归还《{0}》失败：{1}", borrowLog.Book.Title, ex.Message), _growlToken);
                }
            Query();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Query();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _selectedBorrowLogs.Clear();
        }
        public void Handle(AccountModificationEvent message)
        {
            _account = message.Account;
        }
    }
}
