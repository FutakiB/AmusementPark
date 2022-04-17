using System;
using System.Collections.Generic;
using System.Timers;

namespace WeShallNotPass.Model
{
    public class Model
    {
        private MainEntrance mainEntrance;

        #region Properties

        private Item?[,] _gameArea;
        public Item[,] GameArea
        {
            get { return _gameArea; }
            set { _gameArea = value; }
        }

        private int _gameAreaSize;
        public int GameAreaSize
        {
            get { return _gameAreaSize; }
            set { _gameAreaSize = value; }
        }


        private List<Game> _games;
        public List<Game> Games
        {
            get { return _games; }
            set { _games = value; }
        }

        private List<Restaurant> _restaurants;
        public List<Restaurant> Restaurants
        {
            get { return _restaurants; }
            set { _restaurants = value; }
        }

        private List<Restroom> _restrooms;
        public List<Restroom> Restrooms
        {
            get { return _restrooms; }
            set { _restrooms = value; }
        }

        private List<Visitor> _visitors;   
        public List<Visitor> Visitors
        {
            get { return _visitors; }
            set { _visitors = value; }
        }

        private int _money;
        public int Money
        {
            get { return _money; }
            set
            {
                _money = value;
                MoneyUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _isCampaigning;

        public bool IsCampaigning
        {
            get { return _isCampaigning; }
            set { _isCampaigning = value; }
        }
        private int _campaignEnd;

        public int CampaignEnd
        {
            get { return _campaignEnd; }
            set { _campaignEnd = value; }
        }

        private int _time;

        public int Time
        {
            get { return _time; }
            set
            {
                _time = value;
                TimePassed?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                ParkOpenedOrClosed?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Events 

        public event EventHandler<ItemEventArgs> ItemUpdated;
        public event EventHandler<VisitorEventArgs> VisitorUpdated;
        public event EventHandler<VisitorEventArgs> VisitorRemoved;
        public event EventHandler<EventArgs> CampaignUpdated;
        public event EventHandler<EventArgs> TimePassed;
        public event EventHandler<EventArgs> MoneyUpdated;
        public event EventHandler<EventArgs> NewGameStarted;
        public event EventHandler<ErrorMessageEventArgs> ErrorMessageCalled;
        public event EventHandler<EventArgs> ParkOpenedOrClosed;

        #endregion

        #region Methods

        public Model()
        {
            _isOpen = false;
        }

        public void Tick()
        {
            Time++;

            if (IsOpen && Time % 20 == 0)
            {
                RegisterVisitor();
            }
        }

        public void NewGame()
        {
            _gameArea = new Item[GameAreaSize, GameAreaSize];
            _games = new List<Game>();
            _restaurants = new List<Restaurant>();
            _restrooms = new List<Restroom>();
            _visitors = new List<Visitor>();
            _money = 15000;
            MoneyUpdated?.Invoke(this, EventArgs.Empty);
            _isCampaigning = false;
            _time = 0;
            IsOpen = false;
            TimePassed?.Invoke(this, EventArgs.Empty);

            mainEntrance = new MainEntrance(6, 13, "Bejárat", 2, 1, new Uri("/Images/placeholder.png", UriKind.Relative), 0, 0, 5, 40);
            Build(mainEntrance);
        }

        public void OpenPark()
        {
            IsOpen = true;
        }

        public void ClosePark()
        {
            throw new NotImplementedException();
        }

        public void Build(Item item)
        {
            if (!CanBuildAt(item.X, item.Y, item.SizeX, item.SizeY))
            {
                ErrorMessageCalled?.Invoke(this, new ErrorMessageEventArgs("Ezt ide nem tudod letenni!"));
                return;
            }

            if (item.Price > Money)
            {
                ErrorMessageCalled?.Invoke(this, new ErrorMessageEventArgs("Nincs Elég pénzed!"));
                return;
            }

            for (int i = item.X; i < item.X + item.SizeX; i++)
            {
                for (int j = item.Y; j < item.Y + item.SizeY; j++)
                {
                    GameArea[i, j] = item;
                }
            }

            switch (item)
            {
                case Game game:
                    Games.Add(game);
                    break;
                case Restaurant restaurant:
                    Restaurants.Add(restaurant);
                    break;
                case Restroom restroom:
                    Restrooms.Add(restroom);
                    break;
            }

            ItemUpdated?.Invoke(this, new ItemEventArgs(item));
            Money -= item.Price;
        }

        public void Demolish(Item item)
        {
            throw new NotImplementedException();
        }

        private bool CanBuildAt(int x, int y, int sizeX, int sizeY)
        {
            int xEnd = x + sizeX;
            int yEnd = y + sizeY;

            if (xEnd > GameAreaSize || yEnd > GameAreaSize) return false;

            for (int i = x; i < xEnd; i++)
            {
                for (int j = y; j < yEnd; j++)
                {
                    if (GameArea[i, j] != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void RegisterVisitor()
        {
            Random random = new Random();

            int maxCapacity = 0;
            foreach (Game g in Games) maxCapacity += g.MaxCapacity;
            foreach (Restaurant r in Restaurants) maxCapacity += r.MaxCapacity;
            foreach (Restroom r in Restrooms) maxCapacity += r.MaxCapacity;

            if (maxCapacity == 0) return;

            double fullness = Visitors.Count / (double) maxCapacity;

            if (fullness > 1f) return;

            double d1 = mainEntrance.TicketPrice / 100f;
            d1 = d1 * (-1) + 1;

            double d2 = fullness * (-1) + 1;

            double willingness = d1 * d2 * 100;

            if (random.Next(100) > willingness) return;

            string img;
            int imgNumber = random.Next(4);

            if (imgNumber == 0) img = "/Images/characters/red.png";
            else if (imgNumber == 1) img = "/Images/characters/yellow.png";
            else if (imgNumber == 2) img = "/Images/characters/green.png";
            else img = "/Images/characters/cyan.png";

            Visitor v = new Visitor(6 * 64, 13 * 64, 200, 1, 1, 1, new Uri(img, UriKind.Relative));
            Visitors.Add(v);
            VisitorUpdated?.Invoke(this, new VisitorEventArgs(v));

            Money += mainEntrance.TicketPrice;
        }

        private void RemoveVisitor(Visitor v)
        {
            Visitors.Remove(v);
            VisitorRemoved(this, new VisitorEventArgs(v));
        }
        #endregion
    }
}
