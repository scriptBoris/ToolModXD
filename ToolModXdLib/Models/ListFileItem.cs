using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolModXdLib.Models
{
    public class ListFileItem
    {
        /// <summary>
        /// Имеет ли значение разширение файла
        /// </summary>
        public bool IsNoFileExtension { get; set; }

        /// <summary>
        /// Тип значения
        /// </summary>
        public War3Types War3Type { get; set; }

        /// <summary>
        /// Логические значения
        /// </summary>
        public List<string> LogicValues { get; set; } = new List<string>();

        /// <summary>
        /// Значение которое было оригинально указано в commonabilitiesunits.txt или unitUI.slk
        /// </summary>
        public string OriginValue { get; set; }
    }

    public enum War3Types
    {
        Unknown,
        Texture,
        Model
    }
}
