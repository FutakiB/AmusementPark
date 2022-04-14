using System;
using System.Collections.Generic;

namespace WeShallNotPass.Model
{

    public enum VisitorsStatus { AT_ACTIVITY, WALKING, WAITING_IN_QUEUE, WAITING }
    public class Visitor
    {
        #region Fields
        private int _x;
        private int _y;
        private int _money;
        private int _satiety;//low = hungry
        private int _mood;
        private int _restroomNeeds; // low = need a bathroom
        private double _willingness; //around 0.5 (+0.7 /- 0.3)

        private VisitorsStatus _status;
        private Item _destination;
        #endregion

        #region Properties
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }
        public int Money
        {
            get { return _money; }
            set { _money = value; }
        }
        public int Satiety
        {
            get { return _satiety; }
            set { _satiety = value; }
        }
        public int Mood
        {
            get { return _mood; }
            set { _mood = value; }
        }
        public int RestroomNeeds
        {
            get { return _restroomNeeds; }
            set { _restroomNeeds = value; }
        }
        public VisitorsStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
        #endregion

        #region Methods
        public Visitor(int x, int y, int money, int hunger, int mood, int restroomNeeds)
        {
            X = x;
            Y = y;
            Money = money;
            Satiety = hunger;
            Mood = mood;
            RestroomNeeds = restroomNeeds;
            Random rnd = new Random();
            _willingness = 0.5 + (double)rnd.Next(-3, 7) / 10;
            //_destination = destination;
        }

        public void Move()
        {
            throw new NotImplementedException();
        }

        private bool IsGoodDestination(Facility des)
        {
            if (des.IsBuilt && des.IsReachable && des.HasPower)
            {
                if (des is Restroom)
                {
                    return true;
                }
                if (des is Game)
                {
                    if ((double)((Game)des).TicketPrice / 1000 < _willingness && _money >= ((Game)des).TicketPrice)
                    {
                        return true;
                    }
                }
                else if (des is Restaurant)
                {
                    if ((double)((Restaurant)des).FoodPrice / 1000 < _willingness && Money >= ((Restaurant)des).FoodPrice)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void NextActivity(List<Restroom> restrooms, List<Restaurant> restaurants, List<Game> games, MainEntrance mainEntrance)
        {
            if (Mood > 0)
            {
                Random rnd = new Random();
                if (RestroomNeeds < 50 || Satiety < 50)
                {
                    if (RestroomNeeds < Satiety)
                    {
                        Restroom ran = restrooms[rnd.Next(restrooms.Count)];
                        if (IsGoodDestination(ran))
                            _destination = ran;
                    }
                    else
                    {
                        Restaurant ran = restaurants[rnd.Next(restaurants.Count)];
                        if (IsGoodDestination(ran))
                            _destination = ran;
                    }
                }
                else
                {
                    Game ran = games[rnd.Next(games.Count)];
                    if (IsGoodDestination(ran))
                        _destination = ran;
                }
            }
            else
            {
                _destination = mainEntrance;
            }
        }

        private List<Road> FindPath(Item[,] gameArea, int gameAreaSize)
        {
            int[,] edgeColor = new int[gameAreaSize, gameAreaSize];
            Road[,] parent = new Road[gameAreaSize, gameAreaSize];
            List<Road> path = new List<Road>();
            parent[X, Y] = null;
            edgeColor[X, Y] = 1;
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((this.X, this.Y));
            while (queue.Count > 0)
            {
                (int x, int y) = queue.Dequeue();
                if (x + 1 < gameAreaSize)
                {
                    if (gameArea[x + 1, y] == _destination)
                    {
                        Road cur = (Road)gameArea[x, y];
                        while (cur != null)
                        {
                            path.Add(cur);
                            cur = parent[x, y];
                        }
                        return path;
                    }

                    if (gameArea[x + 1, y] is Road && edgeColor[x + 1, y] == 0)
                    {
                        parent[x + 1, y] = (Road)gameArea[x, y];
                        edgeColor[x + 1, y] = 1;
                        queue.Enqueue((x + 1, y));
                    }
                }

                if (x - 1 > 0)
                {
                    if (gameArea[x - 1, y] == _destination)
                    {
                        Road cur = (Road)gameArea[x, y];
                        while (cur != null)
                        {
                            path.Add(cur);
                            cur = parent[x, y];
                        }
                        return path;
                    }

                    if (gameArea[x - 1, y] is Road && edgeColor[x - 1, y] == 0)
                    {
                        parent[x - 1, y] = (Road)gameArea[x, y];
                        edgeColor[x - 1, y] = 1;
                        queue.Enqueue((x - 1, y));
                    }
                }

                if (y + 1 < gameAreaSize)
                {
                    if (gameArea[x, y + 1] == _destination)
                    {
                        Road cur = (Road)gameArea[x, y];
                        while (cur != null)
                        {
                            path.Add(cur);
                            cur = parent[x, y];
                        }
                        return path;
                    }

                    if (gameArea[x, y + 1] is Road && edgeColor[x, y + 1] == 0)
                    {
                        parent[x, y + 1] = (Road)gameArea[x, y];
                        edgeColor[x, y + 1] = 1;
                        queue.Enqueue((x, y + 1));
                    }
                }

                if (y - 1 > 0)
                {
                    if (gameArea[x, y - 1] == _destination)
                    {
                        Road cur = (Road)gameArea[x, y];
                        while (cur != null)
                        {
                            path.Add(cur);
                            cur = parent[x, y];
                        }
                        return path;
                    }

                    if (gameArea[x, y - 1] is Road && edgeColor[x, y - 1] == 0)
                    {
                        parent[x, y - 1] = (Road)gameArea[x, y];
                        edgeColor[x, y - 1] = 1;
                        queue.Enqueue((x, y - 1));
                    }
                }
                edgeColor[x, y] = 2;
            }
            return null;
        }
        #endregion
    }
}
