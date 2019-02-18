using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolModXdLib.Models
{
    public class CellEditor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public object Source { get; set; }

        public string Header { get; set; }

        public List<CellData> Datas { get; set; } = new List<CellData>();

    }

    public class CellData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Key { get; set; }

        public string Value { get; set; }

    }
}
