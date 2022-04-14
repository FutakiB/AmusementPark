using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeShallNotPass.Model;

namespace WeShallNotPass.Model
{

    public enum VisitorsStatus { AT_ACTIVITY, WALKING, WAITING}
    public class Visitor
    {


        #region Properties 
        private int _x;

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        private int _y;

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        private int _money;

        public int Money
        {
            get { return _money; }
            set { _money = value; }
        }

        private int _hunger;

        public int Hunger
        {
            get { return _hunger; }
            set { _hunger = value; }
        }

        private int _mood;

        public int Mood
        {
            get { return _mood; }
            set { _mood = value; }
        }

        private int _restroomNeeds;

        public int RestroomNeeds
        {
            get { return _restroomNeeds; }
            set { _restroomNeeds = value; }
        }

        private VisitorsStatus _status;

        public VisitorsStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }


        #endregion

        #region Fields
        private Item Destination;
        #endregion

        #region Methods
        public Visitor(int x, int y, int money, int hunger, int mood, int restroomNeeds, Item destination)
        {
            X = x;
            Y = y;
            Money = money;
            Hunger = hunger;
            Mood = mood;
            RestroomNeeds = restroomNeeds;
            Destination = destination;
        }

        public void Move()
        {
            throw new NotImplementedException();
        }

        public void NextActivity(List<Restroom> restrooms, List<Restaurant> restaurants,List<Game> games, MainEntrance mainEntrance)
        {
            if (Mood > 0)
            {
                Random rnd = new Random();
                if (RestroomNeeds > 50 || Hunger > 50)
                {
                    if (RestroomNeeds > Hunger)
                    {
                        Destination = restrooms[rnd.Next(restrooms.Count)];
                    }
                    else
                    {
                        Destination = restaurants[rnd.Next(restrooms.Count)];
                    }
                }
                else
                {
                    Destination = games[rnd.Next(games.Count)];
                }
            } else
            {
                Destination = mainEntrance;
            }
        }

        private void FindPath(Item[,] gameArea, int gameAreaSize)
        {
            throw new NotImplementedException();
            int[,] edgeColor = new int[14, 14];
            int[,] parent = new int[14, 14];
            parent[X, Y] = 0;
            Queue<(int, int)> queue = new Queue<(int, int)> ();
            queue.Enqueue((this.X, this.Y));
            while (queue.Count > 0)
            {
                (int x, int y) = queue.Dequeue();
                if ( x+1  < gameAreaSize &&  gameArea[x + 1, y] is Road)
                {

                }
                if (x - 1 > 0 && gameArea[x - 1, y] is Road)
                {

                }
                if (y + 1 < gameAreaSize && gameArea[x, y + 1] is Road)
                {

                }
                if (y - 1 > 0 && gameArea[x, y - 1] is Road)
                {

                }
            }

            /*
             adott locationon megnézi minden fele hogy út van e és
             és ha igen akkor bekerül ka queuba.ű
             
             */
        }
        #endregion


    }
}
