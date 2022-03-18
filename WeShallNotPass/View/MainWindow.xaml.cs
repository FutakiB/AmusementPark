using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeShallNotPass.ViewModel;

namespace WeShallNotPass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox shopitems = sender as ListBox;
            ShopItemViewModel item = shopitems.SelectedItem as ShopItemViewModel;
            if (item == null) return;
            MainViewModel vm = DataContext as MainViewModel;
            vm.SelectedItem = item.obj;

        }
    }
}
