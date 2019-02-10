using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolModXdLib.Models
{
    public class WarSylkProp
    {
        /// <summary>
        /// Column in SLK file
        /// </summary>
        public int Id { get; set; }


        //public int Row { get; set; }

        public string Coordinate { get; set; }
        public string Value { get; set; }

        public WarSylkProp(int id, string value, string coord)
        {
            Id = id;
            Value = value;
            Coordinate = coord;
        }
    }

    public class WarSylkItem
    {
        /// <summary>
        /// Номер строки в SLK файле
        /// </summary>
        public int NumberRow { get; set; }

        /// <summary>
        /// Внутренний идентификатор объектов War3
        /// </summary>
        public string RawCode { get; set; }

        /// <summary>
        /// Строки
        /// </summary>
        public List<WarSylkProp> Data { get; set; } = new List<WarSylkProp>();

        /// <summary>
        /// Системная строка для поддержки работоспособности формата SYLK
        /// </summary>
        public string SystemLine { get; set; }

        public WarSylkItem(string line)
        {
            SystemLine = line;
        }

        public WarSylkItem(int id)
        {
            NumberRow = id;
        }

        public void AddValue(int index, string value, string coordinate)
        {
            if (index == 1)
            {
                RawCode = value;
                Data.Add(new WarSylkProp(index, value, coordinate) );
                return;
            }

            if (Data.Count == 0)
            {
                Data.Add(new WarSylkProp(index, value, coordinate) );
                return;
            }
            else
            {
                var find = Data.FirstOrDefault(x => x.Id == index);
                if (find != null)
                    find.Value = value;
                else
                    Data.Add(new WarSylkProp(index, value, coordinate));
            }
        }

        public override string ToString()
        {
            if (SystemLine != null)
                return SystemLine;
            else
            {
                string result = "";
                int i = 0;
                foreach (var prop in Data)
                {
                    result += $"{prop.Coordinate}{prop.Value}";
                    i++;
                    if (i < Data.Count)
                        result += $"{Environment.NewLine}";
                }
                return result;
            }
        }
    }
}
