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
        private ToolMod _toolMod;

        public ListFileInjector(ToolMod toolMod)
        {
            _toolMod = toolMod;
        }

        public bool Add(string newData)
        {
            var match = Datas.FirstOrDefault( x => string.Equals(newData, x, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                _toolMod.InvokeMessage($"extract listfile: \"{newData}\"");
                Datas.Add(newData);
            }
            else
                return false;
            return true;
        }

        public async void SaveResult(string path)
        {
            using (var sw = File.CreateText(path))
            {
                await sw.WriteLineAsync("(attributes)");
                await sw.WriteLineAsync("(listfile)");
               
                foreach (var line in Datas)
                {
                    await sw.WriteLineAsync(line);
                }
            }
        }
    }
}
