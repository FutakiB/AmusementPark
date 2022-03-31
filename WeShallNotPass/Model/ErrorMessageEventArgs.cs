using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeShallNotPass.Model
{
    public class ErrorMessageEventArgs: EventArgs
    {
        private readonly string _message;

        public ErrorMessageEventArgs(string message)
        {
            this._message = message;
        }

        public string Message
        {
            get { return this._message; }
        }
    }
}
