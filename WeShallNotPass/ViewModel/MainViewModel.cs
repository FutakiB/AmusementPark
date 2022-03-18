using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeShallNotPass.Model;

namespace WeShallNotPass.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private Model.Model _model;
        private Item selectedShopItem;
        private int lastSelectedIndex;
        public Item SelectedItem { 
            get {
                if (lastSelectedIndex == -1) throw new Exception("No shop item was selected.");
                ManageSelection(lastSelectedIndex);
                return selectedShopItem;
            } 
            set {
                selectedShopItem = value;
            }
        }

        public Uri Background { get; set; }

        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<ShopItemViewModel> ShopItems { get; private set; }
        public MainViewModel(Model.Model model)
        {
            _model = model;
            selectedShopItem = null;
            lastSelectedIndex = -1;

            Background = new Uri("/Images/background.png", UriKind.Relative);
            Items = new ObservableCollection<ItemViewModel>();
            ShopItems = new ObservableCollection<ShopItemViewModel>();

            InitShopItems();
        }

        public void InitShopItems()
        {
            ShopItems.Clear();
            ShopItems.Add(new ShopItemViewModel("Hullámvasút", // menu name
                new Uri("/Images/placeholder.png", UriKind.Relative), // picture location
                2, 2, 2600, 50, 100, // sizeX, sizeY, cost, build time, daily fees
                new Game(-1,-1,"Hullámvasút",2,2, "/Images/placeholder.png",2600,50,26,50,30, null, 10, 100,400,30), // type, posX, posY, name, sizeX, sizeY, picture location, price, build time, specifics
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel)))); // select action
        }

        private void ManageSelection(ShopItemViewModel t)
        {
            SelectedItem = t.obj;
            if (lastSelectedIndex != -1) ShopItems.ElementAt(lastSelectedIndex).IsSelected = false;
            lastSelectedIndex = ShopItems.IndexOf(t);
            InitShopItems();
            ShopItems.ElementAt(lastSelectedIndex).IsSelected = true;
            OnPropertyChanged();
        }

        private void ManageSelection(int index)
        {
            InitShopItems();
            selectedShopItem = ShopItems.ElementAt(index).obj;
            ShopItems.ElementAt(index).IsSelected = true;
        }
    }
}
