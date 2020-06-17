using Caliburn.Micro;
using HandyControl.Controls;
using LibraryManagementSystem.Events;
using LibraryManagementSystem.Models.DataBaseModels;
using LibraryManagementSystem.Services;
using System;

namespace LibraryManagementSystem.ViewModels
{
    public class UserInfoViewModel : Screen, IHandle<AccountStateChangedEvent>
    {
        private readonly IEventAggregator _events;
        private readonly UserService _userService;
        private readonly CreditService _creditService;
        private readonly AuthorityService _authorityService;
        private readonly string _growlToken;

        private User _account;

        public string UserName => _account?.UserName;
        public string Name => _account?.Name;
        public string NationalIdentificationNumber => _account?.NationalIdentificationNumber;
        public int? CreditValue => _account?.CreditValue;
        public int AccreditedDays => _account == null ? 0 : _creditService.GetAccreditedDays(_account.CreditValue);
        public string AuthorityLevel => _account == null ? null : _authorityService.IsAdministrator(_account.Id, _account.UserName, _account.Authority) ? "管理员" : "普通会员";

        public UserInfoViewModel(IEventAggregator events, UserService userService, CreditService creditService, AuthorityService authorityService, string growlToken)
        {
            _events = events;
            _events.Subscribe(this);
            _userService = userService;
            _creditService = creditService;
            _authorityService = authorityService;
            _growlToken = growlToken;
        }
        public void Query()
        {
            try
            {
                _account = _userService.Query(_account.Id);
                if (_account == null)
                    throw new Exception("无此用户！");
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => NationalIdentificationNumber);
                NotifyOfPropertyChange(() => CreditValue);
                NotifyOfPropertyChange(() => AccreditedDays);
                NotifyOfPropertyChange(() => AuthorityLevel);
                Growl.Success("账户信息刷新成功！", _growlToken);
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message, _growlToken);
                SignOut();
            }
        }
        public void ChangePassword()
        {
            // TODO
        }
        public void SignOut()
        {
            _events.PublishOnUIThread(new AccountStateChangedEvent(null));
            Growl.Info("账户已退出！", _growlToken);
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
