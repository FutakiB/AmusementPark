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
        private ItemViewModel selectedInfoItem;
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
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand NormalSpeedCommand { get; private set; }
        public DelegateCommand FastSpeedCommand { get; private set; }
        public DelegateCommand CampaignCommand { get; private set; }

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

        public int Time => _model.Time;
        public int CampaignTime => _model.CampaignTime;
        public int Money => _model.Money;
        public int GameCount => _model.Games.Count;
        public int VisitorCount => _model.Visitors.Count;
        public bool IsClosed => !_model.IsOpen;
        public bool CanCampaign => _model.CampaignTime == 0 && _model.IsOpen;
        public Visibility CampaignVisibility => _model.CampaignTime == 0 ? Visibility.Collapsed : Visibility.Visible;

        #endregion

        #region ObservableCollections

        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<ShopItemViewModel> ShopItems { get; private set; }
        public ObservableCollection<InfoItemViewModel> InfoItems { get; private set; }

        #endregion
        
        #region Constructor

        public MainViewModel(Model.Model model)
        {
            _model = model;
            _model.TimePassed += new EventHandler<EventArgs>(timePassed);
            _model.MoneyUpdated += new EventHandler<EventArgs>(moneyUpdated);
            _model.ItemUpdated += _model_ItemUpdated;
            _model.VisitorUpdated += _model_VisitorUpdated;
            _model.VisitorRemoved += _model_VisitorRemoved;
            _model.ErrorMessageCalled += new EventHandler<ErrorMessageEventArgs>(errorMessageCalled);
            _model.ParkOpenedOrClosed += _model_ParkOpenedOrClosed;
            _model.CampaignUpdated += _model_CampaignUpdated;


            NewGameCommand = new DelegateCommand(p => OnNewGame());
            ExitCommand = new DelegateCommand(p => Exit?.Invoke(this, EventArgs.Empty));
            OpenParkCommand = new DelegateCommand(p => model.OpenPark());
            PauseCommand = new DelegateCommand(p => _model.ChangeTimer("stop"));
            NormalSpeedCommand = new DelegateCommand(p => _model.ChangeTimer("normal"));
            FastSpeedCommand = new DelegateCommand(p => _model.ChangeTimer("fast"));
            CampaignCommand = new DelegateCommand(p => _model.StartCampaigning());

            selectedShopItem = null;
            selectedInfoItem = null;
            lastSelectedIndex = -1;

            CanvasClickCommand = new DelegateCommand(args => OnCanvasClick(args as MouseEventArgs));

            Background = new Uri("/Images/background.png", UriKind.Relative);
            Items = new ObservableCollection<ItemViewModel>();
            ShopItems = new ObservableCollection<ShopItemViewModel>();
            InfoItems = new ObservableCollection<InfoItemViewModel>();

            InitShopItems();
            ManageSelection(ShopItems[0]);
        }

        #endregion

        #region Methods

        private void _model_ParkOpenedOrClosed(object sender, EventArgs e)
        {
            OnPropertyChanged("IsClosed");
            OnPropertyChanged("CanCampaign");
        }

        private void errorMessageCalled(object sender, ErrorMessageEventArgs e)
        {
            MessageBox.Show(e.Message, "Vidámpark", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void _model_ItemUpdated(object sender, ItemEventArgs e)
        {
            Item i = e.Item;
            int ind = 0;

            while (ind < Items.Count && Items[ind].Item != i)
            {
                ind++;
            }

            if(ind == Items.Count)
            {
                Items.Add(CreateItemViewModel(i));
                if (selectedInfoItem != null) UpdateInfoPanel();
            }
            else
            {
                Items[ind] = CreateItemViewModel(i);
            }
        }

        private void _model_VisitorUpdated(object sender, VisitorEventArgs e)
        {
            Visitor v = e.Visitor;
            int ind = 0;

            while (ind < Items.Count && Items[ind].Visitor != v)
            {
                ind++;
            }

            if (ind == Items.Count)
            {
                Items.Add(CreateItemViewModel(v));
                OnPropertyChanged("VisitorCount");
            }
            else
            {
                Items[ind] = CreateItemViewModel(v);
            }
        }

        private void _model_VisitorRemoved(object sender, VisitorEventArgs e)
        {
            Visitor v = e.Visitor;
            int ind = 0;

            while (ind < Items.Count && Items[ind].Visitor != v)
            {
                ind++;
            }

            if(ind < Items.Count)
            {
                Items.RemoveAt(ind);
                OnPropertyChanged("VisitorCount");
            }
        }

        private void moneyUpdated(object sender, EventArgs e)
        {
            OnPropertyChanged("Money");
        }


        private void timePassed(object sender, EventArgs e)
        {
            if (selectedInfoItem != null && selectedInfoItem.Item != null && !selectedInfoItem.Item.IsBuilt) UpdateInfoPanel();
            OnPropertyChanged("Time");
        }

        private void _model_CampaignUpdated(object sender, EventArgs e)
        {
            OnPropertyChanged("CampaignTime");
            OnPropertyChanged("CanCampaign");
            OnPropertyChanged("CampaignVisibility");
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
                new Game(-1,-1,"Hullámvasút",3,3, new Uri("/Images/stills/rollercoaster.gif", UriKind.Relative),2600,50,16,50,30, _model.GameArea, 10, 100,400,30), // type, posX, posY, name, sizeX, sizeY, picture location, price, build time, specifics
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel)))); // select action
            ShopItems.Add(new ShopItemViewModel("Körhinta",
                new Uri("/Images/gifs/carousel.gif", UriKind.Relative),
                1, 1, 2200, 25,
                new Game(-1, -1, "Körhinta", 1, 1, new Uri("/Images/stills/carousel.gif", UriKind.Relative), 2200, 25, 8, 30, 20, _model.GameArea, 4, 40, 220, 18),
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Óriáskerék",
                new Uri("/Images/gifs/ferris_wheel.gif", UriKind.Relative),
                2, 2, 3600, 60,
                new Game(-1, -1, "Körhinta", 2, 2, new Uri("/Images/stills/ferris_wheel.gif", UriKind.Relative), 3600, 60, 24, 60, 40, _model.GameArea, 12, 70, 390, 30),
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Kamikaze",
                new Uri("/Images/gifs/roller.gif", UriKind.Relative),
                2, 2,2600, 35,
                new Game(-1, -1, "Kamikaize", 2, 2, new Uri("/Images/stills/roller.gif", UriKind.Relative), 2600, 35, 12, 40, 30, _model.GameArea, 6, 38, 250, 25),
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
                new Restaurant(-1,-1,"Gyorsétterem",2,2, new Uri("/Images/restaurant.png", UriKind.Relative), 1900,40,20,400,10, _model.GameArea, 20,10,50),
                new DelegateCommand(t => ManageSelection(t as ShopItemViewModel))));
            ShopItems.Add(new ShopItemViewModel("Kávézó",
                new Uri("/Images/restaurant2.png", UriKind.Relative),
                2, 2, 1200, 23,
                new Restaurant(-1,-1,"Kávézó",2,2, new Uri("/Images/restaurant2.png", UriKind.Relative), 1200,23,14,260,5, _model.GameArea, 5,5,20),
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
            ItemViewModel item = null;

            // An image was clicked on the canvas
            if (args.OriginalSource is Image img)
            {
                item = (ItemViewModel)img.DataContext;
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

                _model.Build(buildItem);
                OnPropertyChanged("GameCount");
            } else if (item != null) {
                selectedInfoItem = item;
                UpdateInfoPanel();
            }
        }

        private void UpdateInfoPanel()
        {
            InfoItems.Clear();
            if (selectedInfoItem == null) return;
            if (selectedInfoItem.Visitor == null)
            {
                Dictionary<string, int> list = selectedInfoItem.Item.GetInfoPanelItems();
                InfoItems.Add(new InfoItemViewModel(selectedInfoItem.Item.Name, -3, false, selectedInfoItem.Item, -1));
                if (list == null)
                {
                    InfoItems.Add(new InfoItemViewModel("Nincs elérhető információ.", -3, false, selectedInfoItem.Item, -1));
                    return;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    InfoItems.Add(new InfoItemViewModel(list.ElementAt(i).Key, list.ElementAt(i).Value, false, selectedInfoItem.Item, -1));
                }
                list = selectedInfoItem.Item.GetEditableProperty();
                if (selectedInfoItem.Item.GetEditableProperty() != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        InfoItems.Add(new InfoItemViewModel(list.ElementAt(i).Key, list.ElementAt(i).Value, true, selectedInfoItem.Item, i));
                    }
                }
            }
            else
            {

            }
        }

        private void OnNewGame()
        {
            Items.Clear();
            _model.NewGame();

            //NewGame?.Invoke(this, EventArgs.Empty));
        }

        private ItemViewModel CreateItemViewModel(Visitor v)
        {
            return new ItemViewModel("Visitor", v.X + v.DX, v.Y + v.DY, 2, 64, 64, v.Image, v);
        }

        private ItemViewModel CreateItemViewModel(Item i)
        {
            return new ItemViewModel(i.Name, i.X * 64, i.Y * 64, 0, i.SizeX * 64, i.SizeY * 64, i.Image, i);
        }

        #endregion
    }
}
