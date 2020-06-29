using FourOFive.Views.Windows;

namespace FourOFive.Views
{
    public interface IViewsContainer<T>where T : class
    {
       public  void RegisterChildrenView(string key, IChildrenView<T> childrenView);
    }
}
