using Microsoft.UI.Xaml;
using Tinterra.Desktop.Test.ViewModels;

namespace Tinterra.Desktop.Test;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        if (Content is FrameworkElement root)
        {
            root.DataContext = new MainViewModel();
        }
    }
}
