using HandyControl.Controls;
using ReactiveUI;
using System.Windows;
using System.Windows.Media;

namespace FourOFive.Views.Windows
{
    public class ReactiveGlowWindow<TViewModel> :
        GlowWindow, IViewFor<TViewModel>
        where TViewModel : class
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel",
                typeof(TViewModel),
                typeof(ReactiveGlowWindow<TViewModel>),
                new PropertyMetadata(null));

        public ReactiveGlowWindow() : base()
        {
            System.Drawing.Color orangeRed = System.Drawing.Color.OrangeRed;
            System.Drawing.Color yellowGreen = System.Drawing.Color.YellowGreen;
            ActiveGlowColor = Color.FromArgb(orangeRed.A, orangeRed.R, orangeRed.G, orangeRed.B);
            InactiveGlowColor = Color.FromArgb(yellowGreen.A, yellowGreen.R, yellowGreen.G, yellowGreen.B);
        }
        public TViewModel BindingRoot => ViewModel;

        public TViewModel ViewModel
        {
            get => (TViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }
    }
}
