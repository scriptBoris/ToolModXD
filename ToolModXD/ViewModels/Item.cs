using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolModXdGui.ViewModels
{
    public class Cell
    {
        public string Header { get; set; }

        public List<IData> Datas { get; set; } = new List<IData>();
    }

    public interface IData
    {
        string Key { get; set; }

        string Value { get; set; }
    }
}
