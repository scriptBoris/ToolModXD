using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolModXdLib.Models;

namespace ToolModXdLib.Core
{
    public class ListFileInjector
    {
        internal List<string> Datas { get; private set; } = new List<string>();

        public ListFileInjector()
        {

        }

        public bool Add(string newData)
        {
            var match = Datas.FirstOrDefault( x => string.Equals(newData, x, StringComparison.OrdinalIgnoreCase));
            if (match == null)
                Datas.Add(newData);
            else
                return false;
            return true;
        }

        public async void SaveResult(string path)
        {
            using (var sw = File.CreateText(path))
            {
                foreach (var line in Datas)
                {
                    await sw.WriteLineAsync(line);
                }
            }
        }
    }
}
