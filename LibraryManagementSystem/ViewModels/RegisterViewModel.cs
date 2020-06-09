using Caliburn.Micro;
using HandyControl.Controls;
using LibraryManagementSystem.Events;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using System;
using System.ComponentModel.Composition;

namespace LibraryManagementSystem.ViewModels
{
    [Export(typeof(RegisterViewModel))]
    public class RegisterViewModel : Screen
    {
        private readonly UserService _userService;
        private readonly IEventAggregator _events;
        private readonly string _growlToken;

        private string _userNameText;
        private string _passwordText;
        private string _passwordRepeatText;
        private string _nameText;
        private string _nationalIdentificationNumberText;

        public string UserNameText
        {
            set
            {
                _userNameText = value;
                NotifyOfPropertyChange(() => CanRegister);
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
                NotifyOfPropertyChange(() => CanRegister);
                NotifyOfPropertyChange(() => PasswordText);
            }
            private get
            {
                return _passwordText;
            }
        }
        public string PasswordRepeatText
        {
            set
            {
                _passwordRepeatText = value;
                NotifyOfPropertyChange(() => CanRegister);
                NotifyOfPropertyChange(() => PasswordRepeatText);
            }
            private get
            {
                return _passwordRepeatText;
            }
        }
        public string NameText
        {
            set
            {
                _nameText = value;
                NotifyOfPropertyChange(() => CanRegister);
                NotifyOfPropertyChange(() => NameText);
            }
            private get
            {
                return _nameText;
            }
        }
        public string NationalIdentificationNumberText
        {
            set
            {
                _nationalIdentificationNumberText = value;
                NotifyOfPropertyChange(() => CanRegister);
                NotifyOfPropertyChange(() => NationalIdentificationNumberText);
            }
            private get
            {
                return _nationalIdentificationNumberText;
            }
        }

        public RegisterViewModel(IEventAggregator events, UserService userService, string growlToken)
        {
            _events = events;
            _userService = userService;
            _growlToken = growlToken;
        }
        public bool CanRegister
        {
            get
            {
                return !String.IsNullOrEmpty(UserNameText) && !String.IsNullOrEmpty(PasswordText) &&
                    !String.IsNullOrEmpty(PasswordRepeatText) && !String.IsNullOrEmpty(NameText) &&
                    !String.IsNullOrEmpty(NationalIdentificationNumberText);
            }
        }
        public void Register()
        {
            User account = null;
            try
            {
                if (!PasswordText.Equals(PasswordRepeatText))
                    throw new Exception("两次密码不一致！");
                account = _userService.Register(UserNameText, PasswordText, NameText, NationalIdentificationNumberText);
                if (account == null)
                    throw new Exception("未知错误！");
            }
            catch (Exception ex)
            {
                Growl.Error(string.Format("注册失败：{0}", ex.Message), _growlToken);
            }
            Growl.Success("注册成功！", _growlToken);
            UserNameText = PasswordText = PasswordRepeatText = NameText = NationalIdentificationNumberText = null;
            _events.PublishOnUIThread(new AccountStateChangedEvent(account));
        }
        protected override void OnDeactivate(bool close)
        {
            UserNameText = null;
            PasswordText = null;
            PasswordRepeatText = null;
            NameText = null;
            NationalIdentificationNumberText = null;
            base.OnDeactivate(close);
        }
    }
}
