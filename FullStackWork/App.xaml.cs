using System.Configuration;
using System.Data;
using System.Windows;
using FullStackWork.Store;
using FullStackWork.ViewModels;

namespace FullStackWork
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly NavigationStore _navigationStore;
        public App()
        {
            _navigationStore = new NavigationStore();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _navigationStore.CurrentViewModel = new ImagePageVM();
            MainWindow = new MainWindow()
            {
                DataContext = new MainVM(_navigationStore)
            };
            MainWindow.Show();
            base.OnStartup(e);
        }
    }

}
