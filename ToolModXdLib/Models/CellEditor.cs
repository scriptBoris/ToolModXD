using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolModXdLib.Models
{
    public delegate void CellChanged(object sender, object origin, string value);

    public class CellEditor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public object Source { get; set; }

        public string Header { get; set; }

        public ObservableCollection<CellData> Datas { get; set; } = new ObservableCollection<CellData>();

    }

    public class CellData
    {
        private string _value;

        public IData Data { get; set; }

        public string Key { get; set; }

        public string Value {
            get { return _value; }
            set {
                _value = value;

                if (Data != null)
                    Data.Refresh(Key, value);
            }
        }
    }

    public interface IData
    {
        void Refresh(string propname, string newValue);
    }
}
