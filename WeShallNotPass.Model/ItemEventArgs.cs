using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeShallNotPass.Model
{
    public class ItemEventArgs : EventArgs
    {
        public Item Item { get; private set; }

        public ItemEventArgs(Item item)
        {
            Item = item;
        }

    }
}
