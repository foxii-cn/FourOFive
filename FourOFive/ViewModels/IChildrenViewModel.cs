namespace FourOFive.ViewModels
{
    public interface IChildrenViewModel<TParentViewModel> where TParentViewModel : class
    {
        public TParentViewModel ParentViewModel { get; set; }
    }
}
