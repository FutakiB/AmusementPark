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
        private Dictionary<string, int> list;
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
        public bool IsBuilt
        {
            get
            {
                return item.IsBuilt;
            }
            set { }
        }
        public bool IsEditable { get; private set; }
        Model.Item item;
        public InfoItemViewModel(string s, int n, bool ie, Model.Item i, int ind)
        {
            Text = s;
            IsEditable = ie;
            item = i;
            item.ImageChanged += Item_ImageChanged;
            index = ind;
            list = item.GetEditableProperty();
            if (n >= 0) strvalue = n.ToString(); else
            {
                if (n == -3) strvalue = ""; else
                strvalue = (n == -2) ? "igen" : "nem";
            }
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
                    rl.Add(list.ElementAt(i).Value);
                }
            }
            return rl;
        }
    }
}
