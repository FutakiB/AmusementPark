using System;
using System.Collections.Generic;
using System.Timers;

namespace WeShallNotPass.Model
{
    public class Model
    {
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

        private List<Plant> _plants;
        public List<Plant> Plants
        {
            get { return _plants; }
            set { _plants = value; }
        }
        private List<Generator> _generators;

        public List<Generator> Generators
        {
            get { return _generators; }
            set { _generators = value; }
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

        #endregion

        #region Events 

        public event EventHandler<EventArgs> ItemUpdated;
        public event EventHandler<ItemEventArgs> ItemBuilt;
        public event EventHandler<EventArgs> VisitorsUpdated;
        public event EventHandler<EventArgs> CampaignUpdated;
        public event EventHandler<EventArgs> TimePassed;
        public event EventHandler<EventArgs> MoneyUpdated;
        public event EventHandler<EventArgs> NewGameStarted;
        public event EventHandler<ErrorMessageEventArgs> ErrorMessageCalled;
        public event EventHandler<ErrorMessageEventArgs> TimerChanged;

        #endregion

        #region Methods

        public Model()
        {
        }

        public void Tick()
        {
            Time++;

            if (Time % 10 == 0) // a second has passed
            {
                foreach (Visitor v in _visitors) // plants boost visitor's mood
                {
                    int maxPlantBoost = 0;
                    foreach (Plant p in _plants)
                    {
                        if (Math.Abs(v.X - p.X) <= p.Radius && Math.Abs(v.Y - p.Y) <= p.Radius && maxPlantBoost < p.MoodBoost)
                            maxPlantBoost = p.MoodBoost;
                    }
                    v.Mood += maxPlantBoost;
                }

                foreach (Facility f in _restaurants) {
                    if (!f.IsBuilt) {
                        f.BuildTime--;
                        if (f.BuildTime < 0) f.IsBuilt = true;
                    }
                    
                }
                foreach (Facility f in _games) {
                    if (!f.IsBuilt)
                    {
                        f.BuildTime--;
                        if (f.BuildTime < 0) f.IsBuilt = true;
                    }
                }
                foreach (Facility f in _restrooms) {
                    if (!f.IsBuilt)
                    {
                        f.BuildTime--;
                        if (f.BuildTime < 0) f.IsBuilt = true;
                    }
                }
                foreach ( Generator g in _generators)
                {
                    if (!g.IsBuilt)
                    {
                        g.BuildTime--;
                        if (g.BuildTime < 0)
                        {
                            g.IsBuilt = true;
                            foreach (Facility f in _restaurants) f.CheckPower(_gameArea);
                            foreach (Facility f in _games) f.CheckPower(_gameArea);
                            foreach (Facility f in _restrooms) f.CheckPower(_gameArea);
                        }
                    }
                }
            }
        }

        public void NewGame()
        {
            _gameArea = new Item[GameAreaSize, GameAreaSize];
            _games = new List<Game>();
            _restaurants = new List<Restaurant>();
            _restrooms = new List<Restroom>();
            _visitors = new List<Visitor>();
            _plants = new List<Plant>();
            _generators = new List<Generator>();
            _money = 15000;
            MoneyUpdated?.Invoke(this, EventArgs.Empty);
            _isCampaigning = false;
            _time = 0;
            TimePassed?.Invoke(this, EventArgs.Empty);

            Item gen = new MainEntrance(6, 13, "Főbejárat", 2, 1, new Uri("/Images/entrance.png", UriKind.Relative), 0, 0, 5, 400);
            Build(gen);
        }
        public void OpenPark()
        {
            throw new NotImplementedException();
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
                    game.CheckPower(_gameArea);
                    game.CheckRechaibility(_gameArea);
                    break;
                case Restaurant restaurant:
                    Restaurants.Add(restaurant);
                    restaurant.CheckPower(_gameArea);
                    restaurant.CheckRechaibility(_gameArea);
                    break;
                case Restroom restroom:
                    Restrooms.Add(restroom);
                    restroom.CheckPower(_gameArea);
                    restroom.CheckRechaibility(_gameArea);
                    break;
                case Plant p:
                    Plants.Add(p);
                    break;
                case Generator gen:
                    Generators.Add(gen);
                    break;
                case Road r:
                    foreach (Facility f in _restaurants) f.CheckRechaibility(_gameArea);
                    foreach (Facility f in _games) f.CheckRechaibility(_gameArea);
                    foreach (Facility f in _restrooms) f.CheckRechaibility(_gameArea);
                    break;
            }

            ItemBuilt?.Invoke(this, new ItemEventArgs(item));
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

        public void ChangeTimer(string sp)
        {
            TimerChanged.Invoke(this, new ErrorMessageEventArgs(sp));
        }

        private void registerVisitor()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
