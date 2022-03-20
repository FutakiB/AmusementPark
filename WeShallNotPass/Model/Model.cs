using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

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

        private int _money;
        public int Money
        {
            get { return _money; }
            set { _money = value; }
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
            set { _time = value; }
        }

        #endregion

        #region Fields

        private DispatcherTimer _timer;
        #endregion

        #region Events 

        public event EventHandler<EventArgs> ItemUpdated;
        public event EventHandler<EventArgs> VisitorsUpdated;
        public event EventHandler<EventArgs> CampaignUpdated;
        public event EventHandler<EventArgs> TimePassed;
        public event EventHandler<EventArgs> MoneyUpdated;

        #endregion

        #region Methods

        public void NewGame()
        {
            throw new NotImplementedException();
        }
        public void OpenPark()
        {
            throw new NotImplementedException();
        }

        public void ClosePark()
        {
            throw new NotImplementedException();
        }

        public Model()
        {
            GameArea = new Item[14, 14];
            Games = new List<Game>();
            Restaurants = new List<Restaurant>();
            Restrooms = new List<Restroom>();
        }

        public bool Build(Item item)
        {
            if (!CanBuildAt(item.X, item.Y, item.SizeX, item.SizeY))
                return false;

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

            return true;
        }

        public void Demolish(Item item)
        {
            throw new NotImplementedException();
        }

        private bool CanBuildAt(int x, int y, int sizeX, int sizeY)
        {
            for (int i = x; i < x + sizeX; i++)
            {
                for (int j = y; j < y + sizeY; j++)
                {
                    if (GameArea[i, j] != null) return false;
                }
            }

            return true;
        }

        private void registerVisitor()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
