using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WeShallNotPass.Model
{
    public class Model
    {
        private Item[,] _gameArea;
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

        //public List<Restaurant> Restaurants { get; set; }
        private List<Restaurant> _restaurants;
        public List<Restaurant> Restaurants
        {
            get { return _restaurants; }
            set { _restaurants = value; }
        }


        //public List<Restroom> Restrooms { get; set; }
        private List<Restroom> _restrooms;
        public List<Restroom> Restroom
        {
            get { return _restrooms; }
            set { _restrooms = value; }
        }
        //public List<Visitor> Visitors { get; set; }
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

        private DispatcherTimer _timer;





    }
}
