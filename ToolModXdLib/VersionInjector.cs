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
    public class VersionInjector
    {
        public event InjectorMsgHandler EventMessanger;
        public delegate void InjectorMsgHandler(string msg);

        private List<string> _sourceBody = new List<string>();
        private List<string> _targetBody = new List<string>();

        public List<WarGameItem> ListSource { get; private set; } = new List<WarGameItem>();
        public List<WarGameItem> ListTarget { get; private set; } = new List<WarGameItem>();

        public static readonly PropertyInfo[] PropsWar3Obj = typeof(WarGameItem).GetProperties();

        public VersionInjector(string pathSource, string pathTarget)
        {
            using (var sr = new StreamReader(pathSource) )
            {
                string line;
                while( (line = sr.ReadLineAsync().Result) != null)
                {
                    _sourceBody.Add(line);
                }
                sr.Close();
            }

            using (var sr = new StreamReader(pathTarget))
            {
                string line;
                while ((line = sr.ReadLineAsync().Result) != null)
                {
                    _targetBody.Add(line);
                }
                sr.Close();
            }
        }

        public void Objectivation()
        {
            ObjectiveText(_sourceBody, ListSource, false);
            ObjectiveText(_targetBody, ListTarget, true);
        }

        public void Inject()
        {
            foreach (var targetObj in ListTarget)
            {
                var sourceObj = ListSource.FirstOrDefault(x => x.Id == targetObj.Id);
                if (sourceObj == null)
                    continue;

                foreach(var prop in PropsWar3Obj)
                {
                    if (prop.Name == nameof(WarGameItem.Id))
                        continue;

                    if (prop.Name == nameof(WarGameItem.GameBody))
                        continue;

                    if (prop.GetValue(sourceObj) == null)
                        continue;

                    string newValue = (string) prop.GetValue(sourceObj);
                    string oldValue = (string) prop.GetValue(targetObj);
                    if (newValue != oldValue)
                    {
                        prop.SetValue(targetObj, newValue);
                        Echo($"{targetObj.Id} get import: {newValue} old: {oldValue}");
                    }
                }
            }
        }

        public void SaveResult(string dirPath)
        {
            string path = Path.Combine(dirPath, "commonabilitystrings.txt");
            using (var sw = File.CreateText(path))
            {
                foreach (var target in ListTarget)
                {
                    sw.WriteLine(target.ToString());
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

        private void ObjectiveText(List<string> body, List<WarGameItem> list, bool IsNeedUpGameInfo)
        {
            while(body.Count > 0)
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
    }
}
