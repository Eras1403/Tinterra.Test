using Microsoft.UI.Xaml;
using Tinterra.Desktop.Test.ViewModels;

namespace Tinterra.Desktop.Test;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
