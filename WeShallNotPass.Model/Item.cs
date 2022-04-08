using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeShallNotPass.Model
{
    public class Item
    {
        public Item(int x, int y, String name, int sx, int sy, Uri imageLocation, int price, int buildTime)
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
        public Uri Image;
        public int Price;
        public bool IsBuilt;
        public int BuildTime;

        public virtual string UniqueShopString() {
            return "";
        }

        public virtual Dictionary<string, int> GetInfoPanelItems()
        {
            return null;
        }

        public virtual Dictionary<string, int> GetEditableProperty()
        {
            return null;
        }

        public virtual void SetEditableProperty(List<int> l)
        {
            return;
        }
    }

    public class Plant : Item
    {
        public int Radius;
        public int MoodBoost;
        public Plant(int x, int y, String name, int sx, int sy, Uri imageLocation, int price, int buildTime,
            int radius, int mood) : base(x, y, name, sx, sy, imageLocation, price, buildTime)
        {
            Radius = radius;
            MoodBoost = mood;
        }

        public override Dictionary<string, int> GetInfoPanelItems()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            list.Add("Hatáskörnyezet: ", Radius);
            list.Add("Hangulatnövelés: ", MoodBoost);
            return list;
        }

        public override string UniqueShopString() {
            return "Hangulatnövelés: " + MoodBoost + "\nHatáskörnyezet: " + Radius;
        }
    }

    public class Road : Item
    {
        public Road(int x, int y, String name, int sx, int sy, Uri imageLocation, int price, int buildTime)
            : base(x, y, name, sx, sy, imageLocation, price, buildTime)
        {
            
        }
    }

    public class Generator : Item
    {
        public int Radius;
        public Generator(int x, int y, String name, int sx, int sy, Uri imageLocation, int price, int buildTime,
            int rad) : base(x, y, name, sx, sy, imageLocation, price, buildTime)
        {
            Radius = rad;
        }

        public override Dictionary<string, int> GetInfoPanelItems()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            list.Add("Hatáskörnyezet: ", Radius);
            return list;
        }

        public override string UniqueShopString()
        {
            return "Hatáskörnyezet: " + Radius;
        }
    }
    
    public class MainEntrance : Generator
    {
        public int TicketPrice;
        public MainEntrance(int x, int y, String name, int sx, int sy, Uri imageLocation, int price, int buildTime,
            int rad, int tp) : base(x, y, name, sx, sy, imageLocation, price, buildTime, rad)
        {
            TicketPrice = tp;
        }

        public override string UniqueShopString()
        {
            return "Belépő ár: " + TicketPrice;
        }

        public override Dictionary<string, int> GetEditableProperty()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            list.Add("Jegyár: ", TicketPrice);
            return list;
        }

        public override void SetEditableProperty(List<int> l)
        {
            TicketPrice = l[0];
        }
    }

    public class Facility : Item
    {
        public List<Visitor> Queue;
        public int MaxCapacity, RegularFee, Duration;
        public bool HasPower, IsReachable;

        public override string UniqueShopString()
        {
            return "Kapacitás: " + MaxCapacity + "\nNapi költség: " + RegularFee + "\nHasználati idő: " + Duration;
        }
        public Facility(int x, int y, String name, int sx, int sy, Uri imageLocation, int price, int buildTime,
            int capacity, int fee, int duration, Item[,] gamearea) : base(x, y, name, sx, sy, imageLocation, price, buildTime)
        {
            Queue = new List<Visitor>();
            MaxCapacity = capacity;
            RegularFee = fee;
            Duration = duration;
            HasPower = CheckPower(gamearea);
            IsReachable = CheckRechaibility(gamearea);
        }
        public bool CheckPower(Item[,] ga)
        {
            if (ga == null) return false;
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
        bool CheckRechaibility(Item[,] ga)
        {
            if (ga == null) return false;
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
        public override Dictionary<string, int> GetInfoPanelItems()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            list.Add("Kapacitás: ", MaxCapacity);
            list.Add("Várakozók: ", Queue.Count);
            list.Add("Napi költség: ", RegularFee);
            list.Add("Használati idő: ", Duration);
            list.Add("Ellátva árammal: ", HasPower ? -2 : -1);
            list.Add("Elérhető: ", IsReachable ? -2 : -1);
            return list;
        }
        public override string UniqueShopString()
        {
            return "Kapacitás: " + MaxCapacity + "\nNapi költség: " + RegularFee + "\nHasználati idő: " + Duration;
        }
        public Restroom(int x, int y, String name, int sx, int sy, Uri imageLocation, int price, int buildTime,
            int capacity, int fee, int duration, Item[,] gamearea) : base(x, y, name, sx, sy, imageLocation, price, buildTime, capacity, fee, duration, gamearea) { }
    }

    public class Game : Facility
    {
        public int MinCapacity, TicketPrice, OperationCost, MoodBoost;
        private bool isOperating;
        public bool IsOperating
        {
            get { return isOperating; }
            set
            {
                if (value == false)
                {
                    String str = Image.ToString().Substring(13);
                    Image = new Uri("/Images/stills/" + str, UriKind.Relative);
                    isOperating = value;
                } else
                {
                    String str = Image.ToString().Substring(15);
                    Image = new Uri("/Images/gifs/" + str, UriKind.Relative);
                    isOperating = value;
                }
            }
        }
        public override Dictionary<string, int> GetEditableProperty()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            list.Add("Jegyár: ", TicketPrice);
            list.Add("Induláshoz szükséges várakozók száma: ", MinCapacity);
            return list;
        }

        public override void SetEditableProperty(List<int> l)
        {
            TicketPrice = l[0];
            MinCapacity = l[1];
        }

        public override Dictionary<string, int> GetInfoPanelItems()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            list.Add("Kapacitás: ", MaxCapacity);
            list.Add("Várakozók: ", Queue.Count);
            list.Add("Napi költség: ", RegularFee);
            list.Add("Használati idő: ", Duration);
            list.Add("Ellátva árammal: ", HasPower ? -2 : -1);
            list.Add("Elérhető: ", IsReachable ? -2 : -1);
            list.Add("Alkalmi költség: ", OperationCost);
            list.Add("Hangulatnövelés: ", MoodBoost);
            return list;
        }

        public override string UniqueShopString()
        {
            return "Kapacitás: " + MaxCapacity + "\nNapi költség: " + RegularFee + "\nHasználati idő: " + Duration
                + "\nSzükséges kapacitás: " + MinCapacity + "\nJegyár: " + TicketPrice + "\nHasználati költség: " + OperationCost + "\nHangulatnövelés: " + MoodBoost;
        }
        public Game(int x, int y, String name, int sx, int sy, Uri imageLocation, int price, int buildTime,
            int capacity, int fee, int duration, Item[,] gamearea,
            int mincap, int ticketprice, int cost, int mood) : base(x, y, name, sx, sy, imageLocation, price, buildTime, capacity, fee, duration, gamearea)
        {
            MinCapacity = mincap;
            TicketPrice = ticketprice;
            OperationCost = cost;
            MoodBoost = mood;
            IsOperating = true;
        }
    }

    public class Restaurant : Facility
    {
        public int FoodPrice, IngredientCost, HungerBoost;

        public override Dictionary<string, int> GetInfoPanelItems()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            list.Add("Kapacitás: ", MaxCapacity);
            list.Add("Várakozók: ", Queue.Count);
            list.Add("Napi költség: ", RegularFee);
            list.Add("Használati idő: ", Duration);
            list.Add("Alkalmi költség: ", IngredientCost);
            list.Add("Éhségoltás: ", HungerBoost);
            list.Add("Ellátva árammal: ", HasPower ? -2 : -1);
            list.Add("Elérhető: ", IsReachable ? -2 : -1);
            return list;
        }
        public override Dictionary<string, int> GetEditableProperty()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            list.Add("Egy menü ára: ", FoodPrice);
            return list;
        }

        public override void SetEditableProperty(List<int> l)
        {
            FoodPrice = l[0];
        }
        public override string UniqueShopString()
        {
            return "Kapacitás: " + MaxCapacity + "\nNapi költség: " + RegularFee + "\nHasználati idő: " + Duration
                + "\nBeszerzési ár:" + IngredientCost + "\nÉhségnövelés: " + HungerBoost;
        }
        public Restaurant(int x, int y, String name, int sx, int sy, Uri imageLocation, int price, int buildTime,
            int capacity, int fee, int duration, Item[,] gamearea,
            int foodp, int ingrcost, int boost) : base(x, y, name, sx, sy, imageLocation, price, buildTime, capacity, fee, duration, gamearea)
        {
            FoodPrice = foodp;
            IngredientCost = ingrcost;
            HungerBoost = boost;
        }
    }
}