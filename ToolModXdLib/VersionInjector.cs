using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToolModXdLib.Core;
using ToolModXdLib.Models;

namespace ToolModXdLib
{
    internal class VersionInjector : IVersionInjector
    {
        internal static readonly PropertyInfo[] PropsWar3Obj = typeof(WarGameItem).GetProperties();

        private string _fileName;
        private List<WarGameItem> _listTarget;
        private List<WarGameItem> _listSource;
        private List<string> _sourceBody = new List<string>();

        public event InjectorMsgHandler EventMessanger;

        public VersionInjector()
        {
        }

        public void Read(string pathSource)
        {
            _fileName = Path.GetFileName(pathSource);
            using (var sr = new StreamReader(pathSource))
            {
                string line = "";
                while (line != null)
                {
                    line = sr.ReadLine();
                    _sourceBody.Add(line);
                }
                sr.Close();
            }
        }

        public void Objectivation(bool isLoadGameplayData)
        {
            _listSource = new List<WarGameItem>();
            ObjectiveText(_sourceBody, _listSource, isLoadGameplayData);
        }

        /// <summary>
        /// Загрузка целевого файла, в который необходимо закинуть изменения
        /// </summary>
        /// <param name="filePath">Целевой файл</param>
        public void LoadTarget(string filePath)
        {
            _listTarget = new List<WarGameItem>();
            var targetList = new List<string>();
            using (var sr = new StreamReader(filePath))
            {
                string line = "";
                while (line != null)
                {
                    line = sr.ReadLine();
                    targetList.Add(line);
                }
                sr.Close();
            }
            ObjectiveText(targetList, _listTarget, true);
        }

        public void Inject()
        {
            foreach (var targetObj in _listTarget)
            {
                var sourceObj = _listSource.FirstOrDefault(x => x.Id == targetObj.Id);
                if (sourceObj == null)
                    continue;

                foreach (var prop in PropsWar3Obj)
                {
                    if (prop.Name == nameof(WarGameItem.Id))
                        continue;

                    if (prop.Name == nameof(WarGameItem.GameBody))
                        continue;

                    string sourceValue = prop.GetValue(sourceObj) as string;
                    string targetValue = prop.GetValue(targetObj) as string;

                    if (sourceValue == null && targetValue == null)
                        continue;
                    else if (targetValue == null && string.IsNullOrWhiteSpace(sourceValue) == false)
                    {
                        prop.SetValue(targetObj, sourceValue);
                        Echo($"{targetObj.Id} new value:\"{sourceValue}\"");
                        continue;
                    }

                    if (sourceValue != targetValue)
                    {
                        prop.SetValue(targetObj, sourceValue);
                        Echo($"{targetObj.Id} get import:\"{sourceValue}\" old:\"{targetValue}\"");
                    }
                }
            }
        }

        public void SaveResult(string path)
        {
            //string path = Path.Combine(dirPath, _fileName);

            if (_listTarget!= null)
                using (var sw = File.CreateText(path))
                {
                    foreach (var target in _listTarget)
                    {
                        sw.WriteLine(target.ToString());
                    }
                }
            else if (_listSource != null)
                using (var sw = File.CreateText(path))
                {
                    foreach (var src in _listSource)
                    {
                        sw.WriteLine(src.ToString());
                    }
                }
        }

        private void Echo(string msg)
        {
            EventMessanger?.Invoke(msg);
        }

        private const string RegHeader = @"\[....\]";
        private WarGameItem lastWarObj;

        private void ObjectiveText(List<string> body, List<WarGameItem> list, bool IsNeedUpGameInfo)
        {
            lastWarObj = null;
            while (body.Count > 0)
            {
                string line = body.First();

                // RemoveAt не работает, почему то
                body.RemoveAt(0);
                if (line == null)
                    break;

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
        }

        public void GetDataForListfile(ListFileInjector injector)
        {
            foreach (var item in _listSource)
            {
                foreach (var prop in PropsWar3Obj)
                {
                    string value = prop.GetValue(item) as string;
                    if (value != null)
                    {
                        switch(prop.Name)
                        {
                            case nameof(WarGameItem.Art):
                            case nameof(WarGameItem.Unart):
                            case nameof(WarGameItem.ResearchArt):
                            case nameof(WarGameItem.MissileArt):
                            case nameof(WarGameItem.BuffArt):
                            case nameof(WarGameItem.AreaEffectArt):
                            case nameof(WarGameItem.TargetArt):
                            case nameof(WarGameItem.CasterArt):
                            case nameof(WarGameItem.EffectArt):
                            case nameof(WarGameItem.SpecialArt):
                                injector.Add(value);
                                break;
                        }
                    }
                }
            }
        }

        public List<CellEditor> GetCellsForSourceEditor()
        {
            var res = new List<CellEditor>();
            foreach (var item in _listSource)
            {
                res.Add(item.GetCellEditor() );
            }
            return res;
        }

        public List<CellEditor> GetCellsForTargetEditor()
        {
            var res = new List<CellEditor>();
            foreach (var item in _listTarget)
            {
                res.Add(item.GetCellEditor());
            }
            return res;
        }
    }
}
