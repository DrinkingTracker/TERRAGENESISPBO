using Microsoft.UI.Xaml;
using TERRAGENESIS.Pages;

namespace TERRAGENESIS
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RootFrame.Navigate(typeof(Pages.LoginPage));
        }
    }
}