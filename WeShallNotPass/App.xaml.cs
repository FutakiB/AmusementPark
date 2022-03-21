using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            _viewModel.Exit += new EventHandler(ViewModel_Exit);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); // eseménykezelés a bezáráshoz
            _view.Show();
        }

        private void ViewModel_Exit(object sender, EventArgs e)
        {
            _view.Close();
        }

        private void View_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Park", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;

            }
        }
    }
}
