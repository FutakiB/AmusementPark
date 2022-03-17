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
        private Model.Model _model;
        private MainWindow _view;
        private MainViewModel _viewModel;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new Model.Model();
            _model.GameAreaSize = 14;

            _viewModel = new MainViewModel(_model);
            
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();
        }
    }
}
