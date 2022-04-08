﻿using System;
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

        #endregion

        #region Methods

        public Model()
        {
        }

        public void Tick()
        {
            Time++;
        }

        public void NewGame()
        {
            _gameArea = new Item[GameAreaSize, GameAreaSize];
            _games = new List<Game>();
            _restaurants = new List<Restaurant>();
            _restrooms = new List<Restroom>();
            _money = 15000;
            MoneyUpdated?.Invoke(this, EventArgs.Empty);
            _isCampaigning = false;
            _time = 0;
            TimePassed?.Invoke(this, EventArgs.Empty);

            Item gen = new MainEntrance(6, 13, "mainGate", 2, 1, new Uri("/Images/placeholder.png", UriKind.Relative), 0, 0, 5, 400);
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
                    break;
                case Restaurant restaurant:
                    Restaurants.Add(restaurant);
                    break;
                case Restroom restroom:
                    Restrooms.Add(restroom);
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

        private void registerVisitor()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
