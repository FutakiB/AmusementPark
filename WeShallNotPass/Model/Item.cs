using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeShallNotPass.Model
{
    public class Item
    {
        public Item(int x, int y, String name)
        {
            X = x;
            Y = y;
            Name = name;
            SizeX = 1;
            SizeY = 1;
            Image = "";
            Price = 0;
            BuildTime = 0;
            IsBuilt = false;
        }

        public Item(int x, int y, String name, int sx, int sy, String imageLocation, int price, int buildTime)
        {
            X = x;
            Y = y;
            Name = name;
            SizeX = sx;
            SizeY = sy;
            Image = imageLocation;
            Price = price;
            BuildTime = buildTime;
            IsBuilt = false;
        }

        public String Name;
        public int X, Y;
        public int SizeX, SizeY;
        public String Image;
        public int Price, BuildTime;
        public bool IsBuilt;
    }

    public class Plant : Item
    {
        public int Radius;
        public int MoodBoost;
        Plant(int x, int y, String name, int sx, int sy, String imageLocation, int price, int buildTime,
            int radius, int mood) : base(x, y, name, sx, sy, imageLocation, price, buildTime)
        {
            Radius = radius;
            MoodBoost = mood;
        }
    }

    public class Road : Item
    {
        Road(int x, int y, String name, int sx, int sy, String imageLocation, int price, int buildTime)
            : base(x, y, name, sx, sy, imageLocation, price, buildTime)
        {
            
        }
    }

    public class Generator : Item
    {
        public int Radius;
        public Generator(int x, int y, String name, int sx, int sy, String imageLocation, int price, int buildTime,
            int rad) : base(x, y, name, sx, sy, imageLocation, price, buildTime)
        {
            Radius = rad;
        }
    }
    
    public class MainEntrance : Generator
    {
        public int TicketPrice;
        public MainEntrance(int x, int y, String name, int sx, int sy, String imageLocation, int price, int buildTime,
            int rad, int tp) : base(x, y, name, sx, sy, imageLocation, price, buildTime, rad)
        {
            TicketPrice = tp;
        }
    }

    public partial class Visitor { }

    public class Facility : Item
    {
        public List<Visitor> Queue;
        public int MaxCapacity, RegularFee, Duration;
        public bool HasPower, IsReachable;
        public Facility(int x, int y, String name, int sx, int sy, String imageLocation, int price, int buildTime,
            int capacity, int fee, int duration, Item[,] gamearea) : base(x, y, name, sx, sy, imageLocation, price, buildTime)
        {
            Queue = new List<Visitor>();
            MaxCapacity = capacity;
            RegularFee = fee;
            Duration = duration;
            HasPower = CheckPower(gamearea);
            IsReachable = CheckReachibility(gamearea);
        }
        public bool CheckPower(Item[,] ga)
        {
            foreach (Item i in ga)
            {
                if (i is Generator)
                {
                    int xDiff = i.X - X;
                    int yDiff = i.Y - Y;

                    Generator g = i as Generator;

                    if (Math.Abs(xDiff) < g.Radius && Math.Abs(yDiff) < g.Radius)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        bool CheckReachibility(Item[,] ga)
        {
            Item entrance = null;
            //look for entrance to reference position, can be changed to receive as parameter
            //use Item::IsBuilt as a "found" variable for BFS
            foreach (Item i in ga)
            {
                i.IsBuilt = false; // not found yet
                if (i.GetType() == typeof(MainEntrance))
                {
                    entrance = i;
                }
            }
            if (entrance == null) throw new Exception("No entrance found.");
            int height = ga.GetLength(1); // y coords
            int width = ga.GetLength(0); // x coords

            Queue<Item> q = new Queue<Item>();
            q.Enqueue(entrance);
            while (q.Count != 0)
            {
                Item cur = q.Dequeue();
                if (cur == this) return true;
                cur.IsBuilt = true; //found

                int nextX = cur.X - 1, nextY = cur.Y;
                if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
                {
                    if (ga[nextX, nextY].IsBuilt == false &&
                        ga[nextX, nextY].GetType() == typeof(Road)) q.Enqueue(ga[nextX, nextY]);
                }
                nextX = cur.X + 1; nextY = cur.Y;
                if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
                {
                    if (ga[nextX, nextY].IsBuilt == false &&
                        ga[nextX, nextY].GetType() == typeof(Road)) q.Enqueue(ga[nextX, nextY]);
                }
                nextX = cur.X; nextY = cur.Y - 1;
                if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
                {
                    if (ga[nextX, nextY].IsBuilt == false &&
                        ga[nextX, nextY].GetType() == typeof(Road)) q.Enqueue(ga[nextX, nextY]);
                }
                nextX = cur.X; nextY = cur.Y + 1;
                if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
                {
                    if (ga[nextX, nextY].IsBuilt == false &&
                        ga[nextX, nextY].GetType() == typeof(Road)) q.Enqueue(ga[nextX, nextY]);
                }

            }
            
            return false;
        }
    }

    public class Restroom : Facility
    {
        public Restroom(int x, int y, String name, int sx, int sy, String imageLocation, int price, int buildTime,
            int capacity, int fee, int duration, Item[,] gamearea) : base(x, y, name, sx, sy, imageLocation, price, buildTime, capacity, fee, duration, gamearea) { }
    }

    public class Game : Facility
    {
        public int MinCapacity, TicketPrice, OperationCost, MoodBoost;
        public bool isOperating;
        public Game(int x, int y, String name, int sx, int sy, String imageLocation, int price, int buildTime,
            int capacity, int fee, int duration, Item[,] gamearea,
            int mincap, int ticketprice, int cost, int mood) : base(x, y, name, sx, sy, imageLocation, price, buildTime, capacity, fee, duration, gamearea)
        {
            MinCapacity = mincap;
            TicketPrice = ticketprice;
            OperationCost = cost;
            MoodBoost = mood;
            isOperating = false;
        }
    }

    public class Restaurant : Facility
    {
        int FoodPrice, IngredientCost, HungerBoost;
        public Restaurant(int x, int y, String name, int sx, int sy, String imageLocation, int price, int buildTime,
            int capacity, int fee, int duration, Item[,] gamearea,
            int foodp, int ingrcost, int boost) : base(x, y, name, sx, sy, imageLocation, price, buildTime, capacity, fee, duration, gamearea)
        {
            FoodPrice = foodp;
            IngredientCost = ingrcost;
            HungerBoost = boost;
        }
    }
}