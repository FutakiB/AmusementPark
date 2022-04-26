using System;
using System.Windows;

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
        private Uri image;
        public Uri Image
        {
            get
            {
                if (Visitor == null) return Item.Image; else return image;
            }
            set { }
        }
        public bool IsBuilt
        {
            get
            {
                if (Visitor == null) return Item.IsBuilt; else return false;
            }
            set
            {

            }
        }
        public Visibility Visibility { get; set; } = Visibility.Visible;
        public Model.Item Item { get; private set; }
        public Model.Visitor Visitor { get; private set; }

        public ItemViewModel(string name, int x, int y, int z, int sizeX, int sizeY, Uri cimage, Model.Item i)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
            SizeX = sizeX;
            SizeY = sizeY;
            Item = i;
            Item.ImageChanged += Item_ImageChanged;
            image = cimage;
            Visitor = null;
            // Visibility = Visibility.Visible;
        }

        public ItemViewModel(string name, int x, int y, int z, int sizeX, int sizeY, Uri cimage, Model.Visitor v)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
            SizeX = sizeX;
            SizeY = sizeY;
            Item = null;
            image = cimage;
            Visitor = v;
            //Visibility = Visibility.Visible;
        }
        private void Item_ImageChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Image");
            OnPropertyChanged("IsBuilt");
        }
    }
}
