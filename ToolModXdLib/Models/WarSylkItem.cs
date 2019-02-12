﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToolModXdLib.Models
{

    internal class WarSylkColumn
    {
        /// <summary>
        /// Column in SLK file
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Данные ячейки которые были записаны в SLK, например C;X4;Y2
        /// </summary>
        public string Coordinate { get; set; }

        /// <summary>
        /// Значение ячейки, например K"sortUI"
        /// </summary>
        public string Value { get; set; }

        public WarSylkColumn(int id, string value, string coord)
        {
            Id = id;
            Value = value;
            Coordinate = coord;
        }
    }

    internal class WarSylkItem
    {
        /// <summary>
        /// Номер строки в SLK файле
        /// </summary>
        public int NumberRow { get; private set; }

        /// <summary>
        /// Внутренний идентификатор объектов War3 наподобие [U00D]
        /// </summary>
        public string RawCode { get; private set; }

        /// <summary>
        /// Путь к модели, например Units/Custom/WK/WK
        /// </summary>
        public string ModelPath { get; private set; }

        /// <summary>
        /// Значения колонок
        /// </summary>
        public List<WarSylkColumn> Columns { get; private set; } = new List<WarSylkColumn>();

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
                Columns.Add(new WarSylkColumn(index, value, coordinate));
                return;
            }
            else if (index == 3)
            {
                var reg = new Regex("\"(.*)\"");
                var match = reg.Match(value);
                if (match.Success)
                    ModelPath = match.Groups[1].Value;

                Columns.Add(new WarSylkColumn(index, value, coordinate));
                return;
            }

            if (Columns.Count == 0)
            {
                Columns.Add(new WarSylkColumn(index, value, coordinate) );
                return;
            }
            else
            {
                var find = Columns.FirstOrDefault(x => x.Id == index);
                if (find != null)
                    find.Value = value;
                else
                    Columns.Add(new WarSylkColumn(index, value, coordinate));
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
                foreach (var prop in Columns)
                {
                    result += $"{prop.Coordinate}{prop.Value}";
                    i++;
                    if (i < Columns.Count)
                        result += $"{Environment.NewLine}";
                }
                return result;
            }
        }

        public string ToStringConsole()
        {
            if (SystemLine != null)
                return SystemLine;
            else
            {
                string result = $"{NumberRow}) ";
                int i = 0;
                foreach (var prop in Columns)
                {
                    result += $"{prop.Coordinate}{prop.Value}";
                    i++;
                    if (i < Columns.Count)
                        result += $"{Environment.NewLine}";
                }
                return result;
            }
        }
    }
}
