using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
        private DispatcherTimer _timer;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _timer = new DispatcherTimer(DispatcherPriority.Send);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _timer.Tick += _timer_Tick;

            _model = new Model.Model();
            _model.GameAreaSize = 14;
            _model.TimerChanged += _model_TimerChanged;

            _viewModel = new MainViewModel(_model);
            _viewModel.Exit += new EventHandler(ViewModel_Exit);
            _model.NewGame();

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing);
            _view.Show();

            _timer.Start();
        }

        private void _model_TimerChanged(object sender,  Model.ErrorMessageEventArgs e)
        {
            switch (e.Message)
            {
                case "normal":
                    _timer.Start();
                    _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                    break;
                case "fast":
                    _timer.Start();
                    _timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                    break;
                case "stop":
                    _timer.Stop();
                    break;
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _model.Tick();
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
