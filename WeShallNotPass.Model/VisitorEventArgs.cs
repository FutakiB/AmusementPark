using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeShallNotPass.Model
{
    public class VisitorEventArgs : EventArgs
    {
        public Visitor Visitor { get; private set; }

        public VisitorEventArgs(Visitor visitor)
        {
            Visitor = visitor;
        }

    }
}
