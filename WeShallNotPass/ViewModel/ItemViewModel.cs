using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeShallNotPass.ViewModel
{
    class ItemViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public Uri Image { get; set; }
        public bool IsBuilt { get; set; }

        public ItemViewModel(string name, int x, int y, int z, int sizeX, int sizeY, Uri image)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
            SizeX = sizeX;
            SizeY = sizeY;
            Image = image;
        }
    }
}
