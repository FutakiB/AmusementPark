using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeShallNotPass.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        public Uri Background { get; set; }

        public MainViewModel()
        {
            Background = new Uri("Images/background.png", UriKind.Relative);
        }
    }
}
