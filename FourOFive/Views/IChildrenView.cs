namespace FourOFive.Views
{
    public interface IChildrenView<TParentView> where TParentView : class
    {
        public TParentView ParentView { get; set; }
    }
}
