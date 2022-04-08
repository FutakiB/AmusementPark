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
        public Model.Item Item { get; private set; }
        public Model.Visitor Visitor { get; private set; }

        public ItemViewModel(string name, int x, int y, int z, int sizeX, int sizeY, Uri image, Model.Item i)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
            SizeX = sizeX;
            SizeY = sizeY;
            Image = image;
            Item = i;
            Visitor = null;
        }
        
        public ItemViewModel(string name, int x, int y, int z, int sizeX, int sizeY, Uri image, Model.Visitor v)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
            SizeX = sizeX;
            SizeY = sizeY;
            Image = image;
            Item = null;
            Visitor = v;
        }
    }
}
