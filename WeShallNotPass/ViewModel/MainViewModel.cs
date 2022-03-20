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
        #region Fields
        private Model.Model _model;
        private Item selectedShopItem;
        private int lastSelectedIndex;
        #endregion

        #region Events
        public event EventHandler NewGame;
        public event EventHandler Exit;
        public event EventHandler OpenPark;
        #endregion

        #region Commands
        /*public DelegateCommand NewGameCommand { get; private set; } = new DelegateCommand(p =>
        {
            NewGame?.Invoke(p, EventArgs.Empty);
        });
        public DelegateCommand ExitCommand { get; private set; } = new DelegateCommand(p => OnExit());
        public DelegateCommand OpenParkCommand { get; private set; } = new DelegateCommand(p => OnOpenPark());
        //public DelegateCommand CloseParkCommand { get; private set; } = new DelegateCommand(p => OnClosePark());
        */
        #endregion

        #region Properties
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


        public int Time
        {
            get { return _model.Time ; }
            private set {  }
        }

        public int Money
        {
            get { return _model.Money; }
            private set { }
        }




        #endregion

        #region Constructor
        public MainViewModel(Model.Model model)
        {
            _model = model;
            _model.TimePassed += new EventHandler<EventArgs>(timePassed);
            _model.MoneyUpdated += new EventHandler<EventArgs>(moneyUpdated);
            _model.ItemUpdated += new EventHandler<EventArgs>(itemUpdated);
            _model.VisitorsUpdated += new EventHandler<EventArgs>(visitorsUpdated);

            selectedShopItem = null;
            lastSelectedIndex = -1;

            Background = new Uri("/Images/background.png", UriKind.Relative);
            Items = new ObservableCollection<ItemViewModel>();
            ShopItems = new ObservableCollection<ShopItemViewModel>();

            InitShopItems();
        }
        #endregion

        #region Methods
        private void visitorsUpdated(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void itemUpdated(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void moneyUpdated(object sender, EventArgs e)
        {
            OnPropertyChanged("Money");
        }


        public void timePassed(object sender, EventArgs e)
        {
            OnPropertyChanged("Time");
        }

        public void InitShopItems()
        {
            ShopItems.Clear();
            ShopItems.Add(new ShopItemViewModel("Hullámvasút", // menu name
                new Uri("/Images/placeholder.png", UriKind.Relative), // picture location
                2, 2, 2600, 50, // sizeX, sizeY, cost, build time
                new Game(-1,-1,"Hullámvasút",2,2, "/Images/placeholder.png",2600,50,26,50,30, null, 10, 100,400,30), // type, posX, posY, name, sizeX, sizeY, picture location, price, build time, specifics
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel)))); // select action
            ShopItems.Add(new ShopItemViewModel("Pálmafa", // menu name
                new Uri("/Images/placeholder.png", UriKind.Relative), // picture location
                1, 1, 300, 0, // sizeX, sizeY, cost, build time
                new Plant(-1,-1,"Pálmafa",1,1,"", 300,0,5,20), // type, posX, posY, name, sizeX, sizeY, picture location, price, build time, specifics
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Generátor",
                new Uri("/Images/placeholder.png", UriKind.Relative),
                1,1,500,30,
                new Generator(-1,-1,"Generátor",1,1,"",500,30,4),
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Étterem",
                new Uri("/Images/placeholder.png", UriKind.Relative),
                1, 2, 1900, 40,
                new Restaurant(-1,-1,"Étterem",1,2,"", 1900,40,20,400,10,null,20,10,50),
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
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

        #endregion
    }
}
