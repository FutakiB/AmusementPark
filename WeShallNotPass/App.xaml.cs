using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeShallNotPass.ViewModel;

namespace WeShallNotPass
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow view;
        private MainViewModel viewModel;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            viewModel = new MainViewModel();
            
            view = new MainWindow();
            view.DataContext = viewModel;
            view.Show();
        }
    }
}
