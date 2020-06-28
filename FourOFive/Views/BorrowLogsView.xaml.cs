using DynamicData;
using FourOFive.Models.DataBaseModels;
using FourOFive.ViewModels;
using FourOFive.Views.Windows;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FourOFive.Views
{
    public partial class BorrowLogsView : ReactiveUserControl<BorrowLogsViewModel>, IChildrenView<MainWindow>
    {
        public BorrowLogsView(BorrowLogsViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            this.WhenActivated(disposableRegistration =>
            {
                this.BindCommand(ViewModel,
                    vm => vm.SearchCommand,
                    v => v.SearchButton)
                .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                    vm => vm.ReturnBookCommand,
                    v => v.ReturnBookButton)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.SearchResults,
                    v => v.BorrowLogSearchResultDataGrid.ItemsSource)
                .DisposeWith(disposableRegistration);
                ViewModel.WhenAnyValue(vm => vm.IsSearching,
                    vm => vm.SearchResults)
                .Where(issr => issr.Item2 != null)
                .Select(issr => issr.Item1 ? "检索中" : $"共检索到{issr.Item2.Count}条记录")
                .BindTo(this, v => v.SearchStateTextBlock.Text)
                .DisposeWith(disposableRegistration);
                ViewModel.WhenAnyValue(vm => vm.IsSearching,
                    vm => vm.IsReturning,
                    (s, b) => s || b ? Visibility.Visible : Visibility.Collapsed)
                .BindTo(this, v => v.SearchStateLoadingLine.Visibility)
                .DisposeWith(disposableRegistration);

                FilterStateToggleButton.WhenAnyValue(tb => tb.IsChecked)
                .Select(c => c ?? true)
                .Select(c => c ? "显示全部" : "仅未归还")
                .BindTo(FilterStateTextBlock, tb => tb.Text)
                .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    vm => vm.IsAll,
                    v => v.FilterStateToggleButton.IsChecked)
                .DisposeWith(disposableRegistration);

                Observable.FromEventPattern(BorrowLogSearchResultDataGrid, nameof(BorrowLogSearchResultDataGrid.SelectionChanged))
                .Select(ep => ep.EventArgs as SelectionChangedEventArgs)
                .Subscribe(sce =>
                {
                    foreach (object item in sce.RemovedItems)
                    {
                        ViewModel.SelectedReturnableSearchResultsSource.Remove(item as BorrowLog);
                    }
                    foreach (object item in sce.AddedItems)
                    {
                        if (item is BorrowLog borrowLog && borrowLog.GiveBack == null)
                        {
                            ViewModel.SelectedReturnableSearchResultsSource.AddOrUpdate(borrowLog);
                        }
                    }
                })
                .DisposeWith(disposableRegistration);
            });
        }

        private MainWindow parentView;
        public MainWindow ParentView
        {
            get => parentView; set
            {
                parentView = value;
                ViewModel.ParentViewModel = value.ViewModel;
            }
        }
    }
}
