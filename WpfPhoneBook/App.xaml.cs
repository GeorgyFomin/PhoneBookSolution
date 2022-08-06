using System.Windows;
using WpfPhoneBook.ViewModels;

namespace WpfPhoneBook
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainViewModel mainViewModel = new();
            MainWindow mainWindow = new() { DataContext = mainViewModel };
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}
