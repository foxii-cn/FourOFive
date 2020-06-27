using FourOFive.ViewModels;
using FourOFive.Views.Windows;
using HandyControl.Controls;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FourOFive.Views
{
    /// <summary>
    /// BooksView.xaml 的交互逻辑
    /// </summary>
    public partial class BooksView : ReactiveUserControl<BooksViewModel>, IChildrenView<MainWindow>
    {
        public BooksView(BooksViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(ViewModel,
                    vm => vm.SearchTerms,
                    v => v.BooksSearchBar.Text)
                .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                    vm => vm.SearchCommand,
                    v => v.BooksSearchBar,
                    nameof(BooksSearchBar.SearchStarted))
                .DisposeWith(disposableRegistration);
                ViewModel.WhenAnyValue(vm => vm.IsSearching,
                    vm => vm.BookAmount,
                    (s, c) => s ? "检索中" : $"共检索到{c}本书")
                .BindTo(this, v => v.SearchStateTextBlock.Text)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.IsSearching,
                    v => v.SearchStateLoadingLine.Visibility)
                .DisposeWith(disposableRegistration);
                this.WhenAnyValue(v => v.SearchResaultDataGrid.ActualHeight)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(h => ((int)(h - 56)) / 48)
                .DistinctUntilChanged()
                .BindTo(ViewModel,vm=> vm.PageSize)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.SearchResults,
                    v => v.SearchResaultDataGrid.ItemsSource)
                .DisposeWith(disposableRegistration);
                this.Bind(ViewModel,
                    vm => vm.PageIndex,
                    v => v.SearchResaultPagination.PageIndex)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.PageAmount,
                    v => v.SearchResaultPagination.MaxPageCount)
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
