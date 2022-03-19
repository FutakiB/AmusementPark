using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeShallNotPass.Model;

namespace WeShallNotPass.ViewModel
{
    public class ShopItemViewModel : ViewModelBase
    {
        public String Name { get; set; }
        public Uri Image { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int Price { get; set; }
        public int  BuildTime { get; set; }
        public bool IsSelected { get; set; }
        public String UniqueShopText
        {
            get
            {
                if (obj == null) return "";
                return obj.UniqueShopString();
            }
        }
        public Item obj;
        public DelegateCommand SelectCommand { get; set; }

        public ShopItemViewModel(String name, Uri image, int sx, int sy, int price, int bt, Item o, DelegateCommand c)
        {
            Name = name;
            Image = image;
            SizeX = sx;
            SizeY = sy;
            Price = price;
            BuildTime = bt;
            IsSelected = false;
            obj = o;
            SelectCommand = c;
        }
    }
}
