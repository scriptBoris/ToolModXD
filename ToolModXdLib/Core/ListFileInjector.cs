using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public void Add(string innerData)
        {
            // Чистка данных от ""
            var regWraper = new Regex("\\A\"(.*)\"\\Z");
            var regWraperMatch = regWraper.Match(innerData);
            if (regWraperMatch.Success)
                innerData = regWraperMatch.Groups[2].Value;

            // Если данные пришли через Запятую (текстуры)
            var listNewData = new List<string>();
            var split = innerData.Split(',');
            if (split.Length > 0)
                listNewData = new List<string>(split);
            else
                listNewData.Add(innerData);

            foreach (var item in listNewData)
            {
                string newData = item;
                bool isTexture = false;

                // Если данные без расширений
                var regex = new Regex(@"(.*\.mdx|.*\.mdl|.*\.blp|.*\.tga)", RegexOptions.IgnoreCase);
                var regMatch = regex.Match(newData);
                if (regMatch.Success == false)
                {
                    var regexBtn = new Regex(@"ReplaceableTextures.*");
                    var matchBtn = regexBtn.Match(newData);
                    // Если это текстура
                    if (matchBtn.Success)
                        isTexture = true;
                    // Если это модель
                    else
                        isTexture = false;
                }

                // Определяем колизии
                var match = Datas.FirstOrDefault( x => string.Equals(newData, x, StringComparison.OrdinalIgnoreCase));
                if (match == null)
                {
                    _toolMod.InvokeMessage($"extract listfile: \"{newData}\"");
                    if (isTexture)
                    {
                        Datas.Add(newData + ".blp");
                        Datas.Add(newData + ".tga");
                    }
                    else
                    {
                        Datas.Add(newData + ".mdl");
                        Datas.Add(newData + ".mdx");
                    }
                }
            }
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
