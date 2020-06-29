using FourOFive.Models.DataPackages;
using FourOFive.Utilities;
using FourOFive.ViewModels.Windows;
using HandyControl.Controls;
using HandyControl.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace FourOFive.Views.Windows
{
    public partial class MainWindow : ReactiveGlowWindow<MainWindowViewModel>, IViewsContainer<MainWindow>
    {
        private readonly Dictionary<string, IChildrenView<MainWindow>> childrenViews = new Dictionary<string, IChildrenView<MainWindow>>();
        private readonly Dictionary<string, SideMenuItem> sideMenuItems = new Dictionary<string, SideMenuItem>();
        private System.Windows.Window aboutWindow;
        private string activatedKey;
        private readonly string growlToken;
        
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            growlToken = GrowlStackPanel.Name;
            Growl.Register(growlToken, GrowlStackPanel);
            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    vm => vm.AccountAuthorityLevel,
                    v => v.LoginSideMenu.Visibility,
                    value => value == AuthorityLevel.Visitor ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.AccountAuthorityLevel,
                    v => v.RegisterSideMenu.Visibility,
                    value => value == AuthorityLevel.Visitor ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.AccountAuthorityLevel,
                    v => v.BorrowLogSideMenu.Visibility,
                    value => value >= AuthorityLevel.Member ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.AccountAuthorityLevel,
                    v => v.AccountInfoSideMenu.Visibility,
                    value => value >= AuthorityLevel.Member ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.AccountAuthorityLevel,
                    v => v.BooksSideMenu.Visibility,
                    value => value >= AuthorityLevel.Member ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposableRegistration);

                Observable.FromEventPattern(MainSideMenu, nameof(MainSideMenu.SelectionChanged))
                .Select(ep => (ep.EventArgs as FunctionEventArgs<object>)?.Info as SideMenuItem)
                .Where(smi => smi != null)
                .Subscribe(smi => Navigate(smi.Name))
                .DisposeWith(disposableRegistration);

                Observable.FromEventPattern(AboutMenuItem, nameof(AboutMenuItem.Click))
                .Subscribe(ep => aboutWindow.Show())
                .DisposeWith(disposableRegistration);

                foreach (SideMenuItem smi in MainSideMenu.Items.OfType<SideMenuItem>().SelectMany(smi => smi.Items).OfType<SideMenuItem>())
                {
                    sideMenuItems.Add(smi.Name, smi);
                }

                // 注册右上角漂浮通知
                ViewModel.GUINotify.RegisterHandler(async interactioni =>
                {
                    await Task.Run(() =>
                    {
                        GUINotifyingDataPackage info = interactioni.Input;
                        interactioni.SetOutput(Unit.Default);
                        GrowlInfo growlInfo = new GrowlInfo
                        {
                            Message = info.Message,
                            WaitTime = (int)info.Duration.TotalSeconds,
                            Token = growlToken,
                            StaysOpen = false,
                            IsCustom = true
                        };
                        switch (info.Type)
                        {
                            case NotifyingType.Success:
                                growlInfo.WaitTime = growlInfo.WaitTime == 0 ? 4 : growlInfo.WaitTime;
                                Growl.Success(growlInfo);
                                break;
                            default:
                            case NotifyingType.Info:
                                growlInfo.WaitTime = growlInfo.WaitTime == 0 ? 4 : growlInfo.WaitTime;
                                Growl.Info(growlInfo);
                                break;
                            case NotifyingType.Warning:
                                growlInfo.WaitTime = growlInfo.WaitTime == 0 ? 6 : growlInfo.WaitTime;
                                Growl.Warning(growlInfo);
                                break;
                            case NotifyingType.Error:
                                growlInfo.WaitTime = growlInfo.WaitTime == 0 ? 8 : growlInfo.WaitTime;
                                Growl.Error(growlInfo);
                                break;
                            case NotifyingType.Fatal:
                                growlInfo.WaitTime = growlInfo.WaitTime == 0 ? 10 : growlInfo.WaitTime;
                                Growl.Fatal(growlInfo);
                                break;
                        }
                    });
                });
            });
        }

        public void RegisterChildrenView(string key, IChildrenView<MainWindow> childrenView)
        {
            if (key == "AboutWindow" && childrenView is System.Windows.Window about)
            {
                aboutWindow = about;
                aboutWindow.Owner = this;
            }
            else
            {
                childrenViews.Add(key, childrenView);
                childrenView.ParentView = this;
            }
        }
        public void Navigate(string key)
        {
            if (key == activatedKey)
            {
                return;
            }

            if (activatedKey != null)
            {
                sideMenuItems[activatedKey].IsSelected = false;
            }

            sideMenuItems[key].IsSelected = true;
            ActiveItem.Content = childrenViews[key];
            activatedKey = key;
        }
    }
}
