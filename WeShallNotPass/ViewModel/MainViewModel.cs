using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using WeShallNotPass.Model;

namespace WeShallNotPass.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        #region Fields
        private Model.Model _model;
        private Item selectedShopItem;
        private int lastSelectedIndex;
        private bool buildingMode;
        #endregion

        #region Events
        public event EventHandler NewGame;
        public event EventHandler Exit;
        public event EventHandler OpenPark;
        #endregion

        #region Commands
        
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand OpenParkCommand { get; private set; }
        //public DelegateCommand CloseParkCommand { get; private set; } = new DelegateCommand(p => OnClosePark());
        public DelegateCommand CanvasClickCommand { get; private set; }

        #endregion

        #region Properties

        public Item SelectedItem 
        { 
            get 
            {
                if (lastSelectedIndex == -1) throw new Exception("No shop item was selected.");
                ManageSelection(lastSelectedIndex);
                return selectedShopItem;
            } 
            set
            {
                selectedShopItem = value;
            }
        }


        public Uri Background { get; set; }

        public bool BuildingMode
        {
            get => buildingMode;
            set
            {
                buildingMode = value;
                OnPropertyChanged();
            }
        }

        public int Time
        {
            get { return _model.Time; }
            private set { }
        }

        public int Money
        {
            get { return _model.Money; }
            private set { }
        }

        #endregion

        #region ObservableCollections

        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<ShopItemViewModel> ShopItems { get; private set; }

        #endregion
        
        #region Constructor

        public MainViewModel(Model.Model model)
        {
            _model = model;
            _model.TimePassed += new EventHandler<EventArgs>(timePassed);
            _model.MoneyUpdated += new EventHandler<EventArgs>(moneyUpdated);
            _model.ItemUpdated += new EventHandler<EventArgs>(itemUpdated);
            _model.VisitorsUpdated += new EventHandler<EventArgs>(visitorsUpdated);
            _model.ErrorMessageCalled += new EventHandler<ErrorMessageEventArgs>(errorMessageCalled);

            NewGameCommand = new DelegateCommand(p => OnNewGame());
            ExitCommand = new DelegateCommand(p => Exit?.Invoke(this, EventArgs.Empty));
            OpenParkCommand = new DelegateCommand(p => OpenPark?.Invoke(this, EventArgs.Empty));

            selectedShopItem = null;
            lastSelectedIndex = -1;

            CanvasClickCommand = new DelegateCommand(args => OnCanvasClick(args as MouseEventArgs));

            Background = new Uri("/Images/background.png", UriKind.Relative);
            Items = new ObservableCollection<ItemViewModel>();
            ShopItems = new ObservableCollection<ShopItemViewModel>();

            _model.ItemBuilt += _model_ItemBuilt;

            InitShopItems();
            ManageSelection(ShopItems[0]);
        }

        

        #endregion

        #region Methods

        private void _model_ItemBuilt(object sender, ItemEventArgs e)
        {
            Item i = e.Item;
            Items.Add(new ItemViewModel(i.Name, i.X * 64, i.Y * 64, 0, i.SizeX * 64, i.SizeY * 64, i.Image));
        }
        private void errorMessageCalled(object sender, ErrorMessageEventArgs e)
        {
            MessageBox.Show(e.Message, "Vidámpark", MessageBoxButton.OK, MessageBoxImage.Information);
        }

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
            ShopItems.Add(new ShopItemViewModel("Út",
               new Uri("/Images/path.png", UriKind.Relative),
               1, 1, 100, 0,
               new Road(-1,-1,"Út",1,1, new Uri("/Images/path.png", UriKind.Relative), 100,0),
               new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Hullámvasút", // menu name
                new Uri("/Images/gifs/rollercoaster.gif", UriKind.Relative), // picture location
                3, 3, 2600, 50, // sizeX, sizeY, cost, build time
                new Game(-1,-1,"Hullámvasút",3,3, new Uri("/Images/stills/rollercoaster.gif", UriKind.Relative),2600,50,26,50,30, null, 10, 100,400,30), // type, posX, posY, name, sizeX, sizeY, picture location, price, build time, specifics
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel)))); // select action
            ShopItems.Add(new ShopItemViewModel("Körhinta",
                new Uri("/Images/gifs/carousel.gif", UriKind.Relative),
                1, 1, 2200, 25,
                new Game(-1, -1, "Körhinta", 1, 1, new Uri("/Images/stills/carousel.gif", UriKind.Relative), 2200, 25, 12, 30, 20, null, 6, 40, 220, 18),
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Óriáskerék",
                new Uri("/Images/gifs/ferris_wheel.gif", UriKind.Relative),
                2, 2, 3600, 60,
                new Game(-1, -1, "Körhinta", 2, 2, new Uri("/Images/stills/ferris_wheel.gif", UriKind.Relative), 3600, 60, 50, 60, 40, null, 20, 70, 390, 30),
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Kamikaze",
                new Uri("/Images/gifs/roller.gif", UriKind.Relative),
                2, 2,2600, 35,
                new Game(-1, -1, "Kamikaize", 2, 2, new Uri("/Images/stills/roller.gif", UriKind.Relative), 2600, 35, 30, 40, 30, null, 15, 38, 250, 25),
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Pálmafa", // menu name
                new Uri("/Images/palmtree.png", UriKind.Relative), // picture location
                1, 1, 300, 0, // sizeX, sizeY, cost, build time
                new Plant(-1,-1,"Pálmafa",1,1, new Uri("/Images/palmtree.png", UriKind.Relative), 300,0,5,20), // type, posX, posY, name, sizeX, sizeY, picture location, price, build time, specifics
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Generátor",
                new Uri("/Images/generator.png", UriKind.Relative),
                1,1,500,30,
                new Generator(-1,-1,"Generátor",1,1, new Uri("/Images/generator.png", UriKind.Relative), 500,30,4),
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Gyorsétterem",
                new Uri("/Images/restaurant.png", UriKind.Relative),
                2, 2, 1900, 40,
                new Restaurant(-1,-1,"Gyorsétterem",2,2, new Uri("/Images/restaurant.png", UriKind.Relative), 1900,40,20,400,10,null,20,10,50),
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Kávézó",
                new Uri("/Images/restaurant2.png", UriKind.Relative),
                2, 2, 1200, 23,
                new Restaurant(-1,-1,"Kávézó",2,2, new Uri("/Images/restaurant2.png", UriKind.Relative), 1200,23,14,260,5,null,5,5,20),
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

        private void OnCanvasClick(MouseEventArgs args)
        {
            int x;
            int y;

            // An image was clicked on the canvas
            if (args.OriginalSource is Image img)
            {
                ItemViewModel item = (ItemViewModel)img.DataContext;
                x = item.X / 64;
                y = item.Y / 64;
            }
            // The canvas was clicked
            else
            {
                Point position = args.GetPosition(args.Device.Target);
                x = (int)position.X / 64;
                y = (int)position.Y / 64;
            }

            if (BuildingMode)
            {
                // Get a new instance of the selected shop item
                Item buildItem = SelectedItem;
                buildItem.X = x;
                buildItem.Y = y;

                if (!_model.Build(buildItem))
                {
                    //MessageBox.Show("Ezt ide nem tudod letenni", "Vidámpark", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void OnNewGame()
        {
            _model.NewGame();
            //NewGame?.Invoke(this, EventArgs.Empty));
        }

        #endregion
    }
}
