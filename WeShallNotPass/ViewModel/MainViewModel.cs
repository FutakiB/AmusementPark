using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeShallNotPass.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private Model.Model _model;

        public Uri Background { get; set; }

        public ObservableCollection<ItemViewModel> Items { get; private set; }


        public MainViewModel(Model.Model model)
        {
            _model = model;

            Background = new Uri("/Images/background.png", UriKind.Relative);
            Items = new ObservableCollection<ItemViewModel>();
        }
    }
}
