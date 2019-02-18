using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolModXdLib.Models
{
    internal abstract class Warditor
    {
        public virtual CellEditor GetCellEditor()
        {
            throw new NotImplementedException();
        }
    }
}
