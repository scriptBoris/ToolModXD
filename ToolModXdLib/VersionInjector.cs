using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToolModXdLib.Models;

namespace ToolModXdLib
{
    public class VersionInjector : IVersionInjector
    {
        private List<WarGameItem> _listTarget = new List<WarGameItem>();
        private List<string> _sourceBody = new List<string>();

        public event InjectorMsgHandler EventMessanger;
        public List<WarGameItem> GameDataList { get; private set; } = new List<WarGameItem>();
        public static readonly PropertyInfo[] PropsWar3Obj = typeof(WarGameItem).GetProperties();

        public VersionInjector()
        {
        }

        public async Task Read(string pathSource)
        {
            Echo($"Read file: {pathSource}");
            using (var sr = new StreamReader(pathSource))
            {
                string line = await sr.ReadLineAsync();
                while (line != null)
                {
                    _sourceBody.Add(line);
                }
                sr.Close();
            }
        }

        public async Task Objectivation(bool isLoadGameplayData)
        {
            Echo($"Start procedure objectivation...");
            await ObjectiveText(_sourceBody, GameDataList, isLoadGameplayData).ConfigureAwait(false);
        }

        public async Task Inject(List<object> listTargetArg)
        {
            try
            {
                _listTarget = listTargetArg.Cast<WarGameItem>().ToList();
            }
            catch (Exception)
            {
                return;
            }

            foreach (var targetObj in _listTarget)
            {
                var sourceObj = GameDataList.FirstOrDefault(x => x.Id == targetObj.Id);
                if (sourceObj == null)
                    continue;

                foreach (var prop in PropsWar3Obj)
                {
                    if (prop.Name == nameof(WarGameItem.Id))
                        continue;

                    if (prop.Name == nameof(WarGameItem.GameBody))
                        continue;

                    if (prop.GetValue(sourceObj) == null)
                        continue;

                    string newValue = (string)prop.GetValue(sourceObj);
                    string oldValue = (string)prop.GetValue(targetObj);
                    if (newValue != oldValue)
                    {
                        prop.SetValue(targetObj, newValue);
                        Echo($"{targetObj.Id} get import: {newValue} old: {oldValue}");
                    }
                }
            }
        }

        public async Task SaveResult(string dirPath)
        {
            string path = Path.Combine(dirPath, "commonabilitystrings.txt");
            using (var sw = File.CreateText(path))
            {
                foreach (var target in _listTarget)
                {
                    await sw.WriteLineAsync(target.ToString());
                }
            }

            Echo("Result is saved in: " + path);
        }

        private void Echo(string msg)
        {
            EventMessanger?.Invoke(msg);
        }

        private const string RegHeader = @"\[....\]";
        private WarGameItem lastWarObj;

        private async Task ObjectiveText(List<string> body, List<WarGameItem> list, bool IsNeedUpGameInfo)
        {
            await Task.Run(() =>
            {

                while (body.Count > 0)
                {
                    string line = body.First();
                    body.RemoveAt(0);

                    // Find ID
                    var reg = new Regex(RegHeader);
                    var match = reg.Match(line);
                    if (match.Success)
                    {
                        lastWarObj = new WarGameItem(match.Value);
                        list.Add(lastWarObj);
                        Echo($"Object {match.Value}");
                    }
                    else
                    {
                        if (lastWarObj == null)
                            continue;

                        // Find fields
                        var split = line.Split('=');
                        if (split.Length == 0)
                            continue;

                        string bodyField = split[0];
                        var matchField = PropsWar3Obj.FirstOrDefault(x => string.Equals(x.Name, bodyField, StringComparison.OrdinalIgnoreCase));
                        if (matchField != null)
                        {
                            if (matchField.GetValue(lastWarObj) != null)
                                continue;

                            string lineValue = "";
                            for (int j = 1; j <= split.Length - 1; j++)
                                lineValue += split[j];

                            matchField.SetValue(lastWarObj, lineValue);
                        }
                        // Find exceptions fields
                        else if (IsNeedUpGameInfo)
                        {
                            lastWarObj.GameBody.Add(line);
                        }
                    }
                }
            }).ConfigureAwait(false);
        }
    }
}
