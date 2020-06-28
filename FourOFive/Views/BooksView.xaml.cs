using DynamicData;
using FourOFive.Models.DataBaseModels;
using FourOFive.Models.DataPackages;
using FourOFive.ViewModels;
using FourOFive.Views.Windows;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FourOFive.Views
{
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
                ViewModel.WhenAnyValue(
                    vm => vm.IsSearching,
                    vm => vm.IsBorrowing,
                    (s, b) => s || b ? Visibility.Visible : Visibility.Collapsed)
                .BindTo(this, v => v.SearchStateLoadingLine.Visibility)
                .DisposeWith(disposableRegistration);
                this.WhenAnyValue(v => v.SearchResaultDataGrid.ActualHeight)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(h => Math.Max(1, ((int)(h - 56)) / 48))
                .DistinctUntilChanged()
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .BindTo(ViewModel, vm => vm.PageSize)
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

                Observable.FromEventPattern(SearchResaultDataGrid, nameof(SearchResaultDataGrid.SelectionChanged))
                .Select(ep => ep.EventArgs as SelectionChangedEventArgs)
                .Subscribe(sce =>
                {
                    foreach (object item in sce.RemovedItems)
                    {
                        ViewModel.SelectedSearchResultsSource.Remove(item as Book);
                    }
                    foreach (object item in sce.AddedItems)
                    {
                        ViewModel.SelectedSearchResultsSource.AddOrUpdate(item as Book);
                    }
                })
                .DisposeWith(disposableRegistration);
                Observable.FromEventPattern(ChosenToBorrowDataGrid, nameof(ChosenToBorrowDataGrid.SelectionChanged))
                .Select(ep => ep.EventArgs as SelectionChangedEventArgs)
                .Subscribe(sce =>
                {
                    foreach (object item in sce.RemovedItems)
                    {
                        ViewModel.SelectedToBorrowBooksSource.Remove(item as ChosenBookDataPackage);
                    }
                    foreach (object item in sce.AddedItems)
                    {
                        ViewModel.SelectedToBorrowBooksSource.AddOrUpdate(item as ChosenBookDataPackage);
                    }
                })
                .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    vm => vm.AddToBorrowCommand,
                    v => v.AddToBorrowButton)
                .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                    vm => vm.DeleteFromBorrowCommand,
                    v => v.DeleteFromBorrowButton)
                .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.ToBorrowBooks,
                    v => v.ChosenToBorrowDataGrid.ItemsSource)
                .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                    vm => vm.BorrowCommand,
                    v => v.BorrowButton)
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
