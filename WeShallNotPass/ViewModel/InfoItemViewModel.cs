using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeShallNotPass.ViewModel
{
    class InfoItemViewModel : ViewModelBase
    {
        public string Text { get; set; }
        private string strvalue;
        private int index;
        private Dictionary<string, Func<string>> list;
        public string Value { get { return strvalue; } set {
                string instr = value;
                int result;
                bool success = System.Int32.TryParse(instr, out result);
                if (success && result > 0)
                {
                    try
                    {
                        item.SetEditableProperty(convertToList(index, result));
                        strvalue = value;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {

                    }
                }
                OnPropertyChanged();
            }
        }
        public int ValueInt { get {
                int result;
                bool success = System.Int32.TryParse(strvalue, out result);
                if (success) return result; else return 0;
            } set { } }
        public bool IsBuilt
        {
            get
            {
                if (item != null) return item.IsBuilt; else return true;
            }
            set { }
        }
        public bool IsSpecial { get; private set; }
        public bool IsName { get { return index == -2; } set { } }
        public bool IsItem { get
            {
                if (item != null) return true; else return false;
            } set { } 
        }
        private Model.Item item;
        public InfoItemViewModel(string s, Func<string> f, bool ie, Model.Item i, int ind)
        {
            Text = s;
            IsSpecial = ie;
            item = i;
            if (item != null)
            {
                item.ImageChanged += Item_ImageChanged;
                list = item.GetEditableProperty();
            }
            index = ind;
            /*if (n >= 0) strvalue = n.ToString(); else
            {
                if (n == -3) strvalue = ""; else
                strvalue = (n == -2) ? "igen" : "nem";
            }*/
            strvalue = f();
        }

        private void Item_ImageChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("IsBuilt");
        }

        private List<int> convertToList(int ind, int val)
        {
            List<int> rl = new List<int>();
            for (int i=0;i<list.Count;i++)
            {
                if (i == ind)
                {
                    rl.Add(val);
                } else
                {
                    rl.Add(System.Int32.Parse(list.ElementAt(i).Value()));
                }
            }
            return rl;
        }
    }
}
