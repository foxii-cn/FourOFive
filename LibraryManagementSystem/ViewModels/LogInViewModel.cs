using Caliburn.Micro;
using HandyControl.Controls;
using LibraryManagementSystem.Events;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using System;
using System.ComponentModel.Composition;

namespace LibraryManagementSystem.ViewModels
{
    [Export(typeof(LogInViewModel))]
    public class LogInViewModel : Screen
    {
        private readonly UserService _userService;
        private readonly IEventAggregator _events;
        private readonly string _growlToken;

        private string _userNameText;
        private string _passwordText;

        public string UserNameText
        {
            set
            {
                _userNameText = value;
                NotifyOfPropertyChange(() => CanLogIn);
                NotifyOfPropertyChange(() => UserNameText);
            }
            private get
            {
                return _userNameText;
            }
        }
        public string PasswordText
        {
            set
            {
                _passwordText = value;
                NotifyOfPropertyChange(() => CanLogIn);
                NotifyOfPropertyChange(() => PasswordText);
            }
            private get
            {
                return _passwordText;
            }
        }

        public LogInViewModel(IEventAggregator events, UserService userService, string growlToken)
        {
            _events = events;
            _userService = userService;
            _growlToken = growlToken;
        }
        public bool CanLogIn
        {
            get
            {
                return !String.IsNullOrEmpty(UserNameText) && !String.IsNullOrEmpty(PasswordText);
            }
        }
        public void LogIn()
        {
            try
            {
                User account = _userService.LogIn(UserNameText, PasswordText);
                if (account == null)
                    throw new Exception("用户名或密码不正确！");
                Growl.Success("登陆成功！", _growlToken);
                UserNameText = PasswordText = null;
                _events.PublishOnUIThread(new AccountStateChangedEvent(account));
            }
            catch (Exception ex)
            {
                Growl.Error(String.Format("登陆失败：{0}", ex.Message), _growlToken);
            }
        }
        protected override void OnDeactivate(bool close)
        {
            UserNameText = null;
            PasswordText = null;
            base.OnDeactivate(close);
        }
    }
}
