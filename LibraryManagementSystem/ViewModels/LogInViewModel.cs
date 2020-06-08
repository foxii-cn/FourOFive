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
            User account = null;
            try
            {
                account = _userService.LogIn(UserNameText, PasswordText);
                if (account == null)
                {
                    Growl.Error("用户名或密码不正确！", _growlToken);
                }
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message, _growlToken);
            }
            if (account != null)
            {
                Growl.Success("登陆成功！", _growlToken);
                UserNameText = PasswordText = "";
                _events.PublishOnUIThread(new AccountModificationEvent(account));
            }
        }
    }
}
