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
        internal List<ListFileItem> Datas { get; private set; } = new List<ListFileItem>();
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
            var listNewData = new List<ListFileItem>();
            var split = innerData.Split(',');

            if (split.Length == 0)
            {
                listNewData.Add(new ListFileItem { OriginValue = innerData });
            }
            else
            {
                foreach (var item in split)
                {
                    listNewData.Add(new ListFileItem
                    {
                        OriginValue = item
                    });
                }
            }

            foreach (var item in listNewData)
            {
                string newData = item.OriginValue;

                var regex = new Regex(@"(.*\.mdx|.*\.mdl|.*\.blp|.*\.tga)", RegexOptions.IgnoreCase);
                var regMatch = regex.Match(newData);
                // Если данные без расширений
                if (regMatch.Success == false)
                {
                    item.IsNoFileExtension = true;
                    var regexBtn = new Regex(@"ReplaceableTextures.*");
                    var matchBtn = regexBtn.Match(newData);
                    // Если это текстура
                    if (matchBtn.Success)
                        item.War3Type = War3Types.Texture;
                    // Если это модель
                    else
                        item.War3Type = War3Types.Model;
                }

                // Определяем колизии
                var match = Datas.FirstOrDefault( x => string.Equals(newData, x.OriginValue, StringComparison.OrdinalIgnoreCase));
                if (match == null)
                {
                    // Если файл без расширения
                    if (item.IsNoFileExtension)
                    {
                        if (item.War3Type == War3Types.Texture)
                        {
                            item.LogicValues.Add(newData + ".blp");
                            item.LogicValues.Add(newData + ".tga");
                        }
                        else
                        {
                            item.LogicValues.Add(newData + ".mdx");
                            item.LogicValues.Add(newData + ".mdl");
                        }
                    }
                    else
                        // Если файл имеет расширение - просто добавляем
                        item.LogicValues.Add(newData);

                    Datas.Add(item);
                    _toolMod.InvokeMessage($"extract listfile: \"{newData}\"");
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
                    foreach (var item in line.LogicValues)
                    {
                        await sw.WriteLineAsync(item);
                    }
                }
            }
        }
    }
}
