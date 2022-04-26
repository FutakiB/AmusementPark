using System;
using System.Collections.Generic;

namespace WeShallNotPass.Model
{

    public enum VisitorsStatus { AT_ACTIVITY, WALKING, WAITING_IN_QUEUE, WAITING }
    public class Visitor
    {
        #region Fields
        private List<Road> _path;
        private Item _destination;
        #endregion

        #region Events
        public event EventHandler<VisitorEventArgs> VisitorArrived;
        #endregion

        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int DX { get; private set; }
        public int DY { get; private set; }
        public int Money { get; set; }
        public int Satiety { get; set; }
        public int Mood { get; set; }
        public int RestroomNeeds { get; set; }
        public Uri Image { get; set; }
        public int AtActivitiTime { get; set; }

        public Item Destination
        {
            get { return _destination; }
            private set { _destination = value; }
        }


        public VisitorsStatus Status { get; set; }
        #endregion

        #region Methods
        public Visitor(int x, int y, int money, int hunger, int mood, int restroomNeeds, Uri image)
        {
            X = x;
            Y = y;
            Money = money;
            Satiety = hunger;
            Mood = mood;
            RestroomNeeds = restroomNeeds;
            Image = image;
            Status = VisitorsStatus.WAITING;

            Random rnd = new Random();

            DX = rnd.Next(-20, 20);
            DY = rnd.Next(-20, 20);
        }

        public void VisitorTick(List<Restroom> restrooms, List<Restaurant> restaurants, List<Game> games, MainEntrance mainEntrance, Item[,] gameArea, int gameAreaSize)
        {
            switch (Status)
            {
                case VisitorsStatus.WAITING:
                    NextActivity(restrooms, restaurants, games, mainEntrance, gameArea, gameAreaSize);
                    break;
                case VisitorsStatus.AT_ACTIVITY:
                    //TODO: mood increas
                    break;
                case VisitorsStatus.WAITING_IN_QUEUE:
                    //TODO: mood decreas
                    break;
                case VisitorsStatus.WALKING:
                    Move();
                    break;
                default:
                    return;
            }
        }

        private void Move()
        {
            int stepDistance = 1;
            if (_path.Count > 0)
            {
                Road next = _path[_path.Count - 1];
                if (X < next.X * 64)
                {
                    X += stepDistance;
                }
                if (Y < next.Y * 64)
                {
                    Y += stepDistance;
                }
                if (X > next.X * 64)
                {
                    X -= stepDistance;
                }
                if (Y > next.Y * 64)
                {
                    Y -= stepDistance;
                }
                if (X == next.X * 64 && Y == next.Y * 64)
                {
                    _path.RemoveAt(_path.Count - 1);

                }
            }
            else
            {
                Status = VisitorsStatus.WAITING_IN_QUEUE;
                VisitorArrived?.Invoke(this, new VisitorEventArgs(this));
            }
        }

        private bool IsGoodDestination(Facility des)
        {
            if (des.IsBuilt && des.IsReachable && des.HasPower)
            {
                if (des is Restroom)
                {
                    return true;
                }
                Random rnd = new Random();
                if (des is Game && rnd.Next(0, 1000) < ((Game)des).TicketPrice)
                {
                    if (Money >= ((Game)des).TicketPrice)
                    {
                        return true;
                    }
                }
                else if (des is Restaurant && rnd.Next(0, 1000) < ((Restaurant)des).FoodPrice)
                {
                    if (Money >= ((Restaurant)des).FoodPrice)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void NextActivity(List<Restroom> restrooms, List<Restaurant> restaurants, List<Game> games, MainEntrance mainEntrance, Item[,] gameArea, int gameAreaSize)
        {
            _destination = null;
            if (Mood <= 0)
            {
                _destination = mainEntrance;
            }
            else
            {
                Random rnd = new Random();
                if (RestroomNeeds < 50 && restrooms.Count > 0)
                {
                    Restroom ran = restrooms[rnd.Next(restrooms.Count)];
                    if (IsGoodDestination(ran))
                        _destination = ran;
                }
                else if (Satiety < 50 && restaurants.Count > 0)
                {
                    Restaurant ran = restaurants[rnd.Next(restaurants.Count)];
                    if (IsGoodDestination(ran))
                        _destination = ran;
                }
                else if (games.Count > 0)
                {
                    Game ran = games[rnd.Next(games.Count)];
                    if (IsGoodDestination(ran))
                        _destination = ran;
                }
                if (_destination == null)
                    Status = VisitorsStatus.WAITING;
                else
                {
                    _path = FindPath(gameArea, gameAreaSize);
                    Status = VisitorsStatus.WALKING;
                }
            }
        }

        private List<Road> FindPath(Item[,] gameArea, int gameAreaSize)
        {
            int[,] edgeColor = new int[gameAreaSize, gameAreaSize];
            Road[,] parent = new Road[gameAreaSize, gameAreaSize];
            List<Road> path = new List<Road>();
            parent[X / 64, Y / 64] = null;
            edgeColor[X / 64, Y / 64] = 1;
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((this.X / 64, this.Y / 64));
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
                            cur = parent[cur.X, cur.Y];
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
                            cur = parent[cur.X, cur.Y];

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
                            cur = parent[cur.X, cur.Y];

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
                            cur = parent[cur.X, cur.Y];

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

        public Dictionary<string, Func<string>> GetInfoPanelItems()
        {
            Dictionary<string, Func<string>> list = new Dictionary<string, Func<string>>();
            list.Add("Jóllakottság: ", () => { return Satiety.ToString(); });
            list.Add("Kedv: ", () => { return Mood.ToString(); });
            list.Add("WC: ", () => { return RestroomNeeds.ToString(); });
            list.Add("Pénz: ", () => { return Money.ToString(); });
            list.Add("Státusz: ", () =>
            {
                string val = "";
                switch (Status)
                {
                    case VisitorsStatus.AT_ACTIVITY:
                        val = "Tevékenykedik";
                        break;
                    case VisitorsStatus.WALKING:
                        val = "Sétál";
                        break;
                    case VisitorsStatus.WAITING_IN_QUEUE:
                        val = "Sorban áll";
                        break;
                    case VisitorsStatus.WAITING:
                        val = "Várakozik";
                        break;
                }
                return val;
            });
            list.Add("Cél: ", () => { if (_destination != null) return _destination.Name; else return "nincs"; });
            return list;
        }
        #endregion
    }
}
