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
            originalUri = Image.ToString();
            Price = price;
            BuildTime = buildTime;
            if (buildTime <= 0) isBuilt = true; else IsBuilt = false;
        }

        protected string originalUri;
        public String Name;
        public int X, Y;
        public int SizeX, SizeY;
        private Uri image;
        public Uri Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                ImageChanged?.Invoke(this, new EventArgs());
            }
        }
        public int Price;
        private bool isBuilt;
        public bool IsBuilt { get { return isBuilt; } set
            {
                isBuilt = value;
                if (isBuilt == false) Image = new Uri("/Images/gifs/construction.gif", UriKind.Relative);
                    else Image = new Uri(originalUri, UriKind.Relative);
            }
        }
        public int BuildTime { get; set; }

        public event EventHandler<EventArgs> ImageChanged;

        public virtual string UniqueShopString() {
            return "";
        }

        public virtual Dictionary<string, Func<string>> GetInfoPanelItems()
        {
            return null;
        }

        public virtual Dictionary<string, Func<string>> GetEditableProperty()
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

        public override Dictionary<string, Func<string>> GetInfoPanelItems()
        {
            Dictionary<string, Func<string>> list = new Dictionary<string, Func<string>>();
            if (!IsBuilt) list.Add("Hátralévő építési idő: ", () => { return BuildTime.ToString(); });
            list.Add("Hatáskörnyezet: ", () => { return Radius.ToString(); });
            list.Add("Hangulatnövelés: ", () => { return MoodBoost.ToString(); });
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

        public override Dictionary<string, Func<string>> GetInfoPanelItems()
        {
            Dictionary<string, Func<string>> list = new Dictionary<string, Func<string>>();
            if (!IsBuilt) list.Add("Hátralévő építési idő: ", () => { return BuildTime.ToString(); });
            list.Add("Hatáskörnyezet: ", () => { return Radius.ToString(); });
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

        public override Dictionary<string, Func<string>> GetEditableProperty()
        {
            Dictionary<string, Func<string>> list = new Dictionary<string, Func<string>>();
            list.Add("Jegyár: ", () => { return TicketPrice.ToString(); });
            return list;
        }

        public override void SetEditableProperty(List<int> l)
        {
            TicketPrice = l[0];
        }
    }

    public class Facility : Item
    {
        public Queue<Visitor> VisitorQueue;
        public int MaxCapacity, RegularFee, Duration;
        public bool HasPower, IsReachable;

        public override string UniqueShopString()
        {
            return "Kapacitás: " + MaxCapacity + "\nNapi költség: " + RegularFee + "\nHasználati idő: " + Duration;
        }
        public Facility(int x, int y, String name, int sx, int sy, Uri imageLocation, int price, int buildTime,
            int capacity, int fee, int duration, Item[,] gamearea) : base(x, y, name, sx, sy, imageLocation, price, buildTime)
        {
            VisitorQueue = new Queue<Visitor>();
            MaxCapacity = capacity;
            RegularFee = fee;
            Duration = duration;
            HasPower = false;
            IsReachable = false;
        }
        public void CheckPower(Item[,] ga)
        {
            foreach (Item i in ga)
            {
                if (i is Generator)
                {
                    int xDiff = i.X - X;
                    int yDiff = i.Y - Y;

                    Generator g = i as Generator;

                    if (Math.Abs(xDiff) < g.Radius && Math.Abs(yDiff) < g.Radius && g.IsBuilt)
                    {
                        HasPower = true;
                    }
                }
            }
        }
        public void CheckRechaibility(in Item[,] ga)
        {
            Item entrance = null;
            //look for entrance to reference position, can be changed to receive as parameter
            //use Item::IsBuilt as a "found" variable for BFS
            int height = ga.GetLength(1); // y coords
            int width = ga.GetLength(0); // x coords
            bool[,] Found = new bool[height, width];
            for (int i = 0; i < height; i++) for (int j = 0; j < width; j++) Found[i, j] = false;
            foreach (Item i in ga)
            {
                if (i != null && i.GetType() == typeof(MainEntrance))
                {
                    entrance = i;
                    break;
                }
            }
            if (entrance == null) throw new Exception("No entrance found.");

            Queue<Item> q = new Queue<Item>();
            q.Enqueue(entrance);
            while (q.Count != 0)
            {
                Item cur = q.Dequeue();
                if (cur == this)
                {
                    IsReachable = true;
                    return;
                }
                for (int i=cur.X;i<cur.X+cur.SizeX;i++) for (int j=cur.Y;j<cur.Y+cur.SizeY;j++) Found[i, j] = true; //found
                if (cur.GetType() != typeof(Road) && cur.GetType() != typeof(MainEntrance)) continue;

                int nextX = cur.X - 1, nextY = cur.Y;
                if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height && ga[nextX, nextY] != null)
                {
                    if (Found[nextX, nextY] == false) q.Enqueue(ga[nextX, nextY]);
                }
                nextX = cur.X + 1; nextY = cur.Y;
                if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height && ga[nextX, nextY] != null)
                {
                    if (Found[nextX, nextY] == false) q.Enqueue(ga[nextX, nextY]);
                }
                nextX = cur.X; nextY = cur.Y - 1;
                if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height && ga[nextX, nextY] != null)
                {
                    if (Found[nextX, nextY] == false) q.Enqueue(ga[nextX, nextY]);
                }
                nextX = cur.X; nextY = cur.Y + 1;
                if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height && ga[nextX, nextY] != null)
                {
                    if (Found[nextX, nextY] == false) q.Enqueue(ga[nextX, nextY]);
                }

            }
        }
    }

    public class Restroom : Facility
    {
        public override Dictionary<string, Func<string>> GetInfoPanelItems()
        {
            Dictionary<string, Func<string>> list = new Dictionary<string, Func<string>>();
            if (!IsBuilt) list.Add("Hátralévő építési idő: ", () => { return BuildTime.ToString(); });
            list.Add("Kapacitás: ", () => { return MaxCapacity.ToString(); });
            list.Add("Várakozók: ", () => { return VisitorQueue.Count.ToString(); });
            list.Add("Napi költség: ", () => { return RegularFee.ToString(); });
            list.Add("Használati idő: ", () => { return Duration.ToString(); });
            list.Add("Ellátva árammal: ", () => { if (HasPower) return "igen"; else return "nem"; });
            list.Add("Elérhető: ", () => { if (IsReachable) return "igen"; else return "nem"; });
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
                isOperating = value;
                if (IsBuilt)
                {
                    if (value == false)
                    {
                        String str = Image.ToString().Substring(13);
                        Image = new Uri("/Images/stills/" + str, UriKind.Relative);
                    } else
                    {
                        String str = Image.ToString().Substring(15);
                        Image = new Uri("/Images/gifs/" + str, UriKind.Relative);
                    } 
                }
                
            }
        }
        public override Dictionary<string, Func<string>> GetEditableProperty()
        {
            Dictionary<string, Func<string>> list = new Dictionary<string, Func<string>>();
            list.Add("Jegyár: ", () => { return TicketPrice.ToString(); });
            list.Add("Induláshoz szükséges várakozók száma: ", () => { return MinCapacity.ToString(); });
            return list;
        }

        public override void SetEditableProperty(List<int> l)
        {
            TicketPrice = l[0];
            if (l[1] > MaxCapacity) throw new ArgumentOutOfRangeException("Minimum capacity cannot be higher than max capacity.");
            MinCapacity = l[1];
        }

        public override Dictionary<string, Func<string>> GetInfoPanelItems()
        {
            Dictionary<string, Func<string>> list = new Dictionary<string, Func<string>>();
            if (!IsBuilt) list.Add("Hátralévő építési idő: ", () => { return BuildTime.ToString(); });
            list.Add("Kapacitás: ", () => { return MaxCapacity.ToString(); });
            list.Add("Várakozók: ", () => { return VisitorQueue.Count.ToString(); });
            list.Add("Napi költség: ", () => { return RegularFee.ToString(); });
            list.Add("Használati idő: ", () => { return Duration.ToString(); });
            list.Add("Ellátva árammal: ", () => { if (HasPower) return "igen"; else return "nem"; });
            list.Add("Elérhető: ", () => { if (IsReachable) return "igen"; else return "nem"; });
            list.Add("Alkalmi költség: ", () => { return OperationCost.ToString(); });
            list.Add("Hangulatnövelés: ", () => { return MoodBoost.ToString(); });
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

        public override Dictionary<string, Func<string>> GetInfoPanelItems()
        {
            Dictionary<string, Func<string>> list = new Dictionary<string, Func<string>>();
            if (!IsBuilt) list.Add("Hátralévő építési idő: ", () => { return BuildTime.ToString(); });
            list.Add("Kapacitás: ", () => { return MaxCapacity.ToString(); });
            list.Add("Várakozók: ", () => { return VisitorQueue.Count.ToString(); });
            list.Add("Napi költség: ", () => { return RegularFee.ToString(); });
            list.Add("Használati idő: ", () => { return Duration.ToString(); });
            list.Add("Alkalmi költség: ", () => { return IngredientCost.ToString(); });
            list.Add("Éhségoltás: ", () => { return HungerBoost.ToString(); });
            list.Add("Ellátva árammal: ", () => { if (HasPower) return "igen"; else return "nem"; });
            list.Add("Elérhető: ", () => { if (IsReachable) return "igen"; else return "nem"; });
            return list;
        }
        public override Dictionary<string, Func<string>> GetEditableProperty()
        {
            Dictionary<string, Func<string>> list = new Dictionary<string, Func<string>>();
            list.Add("Egy menü ára: ", () => { return FoodPrice.ToString(); });
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