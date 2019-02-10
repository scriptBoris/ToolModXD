using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToolModXdLib.Models;

namespace ToolModXdLib
{
    public class VersionInjectorSlk
    {
        public event InjectorMsgHandler EventMessanger;
        public delegate void InjectorMsgHandler(string msg);

        private List<string> _sourceBody = new List<string>();
        private List<string> _targetBody = new List<string>();

        public List<WarSylkItem> ListSource { get; private set; } = new List<WarSylkItem>();
        public List<WarSylkItem> ListTarget { get; private set; } = new List<WarSylkItem>();

        public VersionInjectorSlk(string pathSource, string pathTarget)
        {
            using (var sr = new StreamReader(pathSource))
            {
                string line;
                while ((line = sr.ReadLineAsync().Result) != null)
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
            while (ListSource.Count > 0)
            {
                var source = ListSource.First();
                ListSource.RemoveAt(0);

                if (source.NumberRow == 1)
                    continue;

                if (source.SystemLine == null)
                {
                    var target = ListTarget.FirstOrDefault(x => x.RawCode == source.RawCode);
                    if (target != null)
                    {
                        Import(source, target);
                    }
                }
            }
        }

        public void SaveResult(string dirPath)
        {
            string path = Path.Combine(dirPath, "UnitUI.slk");
            using (var sw = File.CreateText(path))
            {
                foreach (var target in ListTarget)
                {
                    sw.WriteLine(target.ToString());
                }
            }

            Echo("Result is saved in: " + path);
        }

        private void Import(WarSylkItem from, WarSylkItem target)
        {
            foreach(var targetItem in target.Data)
            {
                if (target.RawCode == "K\"Nfir\"")
                    Console.WriteLine("");

                if (targetItem.Coordinate == "C;X38")
                    Console.WriteLine("");

                switch (targetItem.Id)
                {
                    case 3:
                    case 5:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 29:
                    case 30:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                    case 46:
                    case 47:
                        break;
                    default:
                        continue;
                }

                var fromItem = from.Data.FirstOrDefault(x => x.Id == targetItem.Id);
                if (fromItem != null && targetItem.Value != fromItem.Value)
                {
                    Echo($"{target.RawCode} get import {fromItem.Value}, old: {targetItem.Value}");
                    targetItem.Value = fromItem.Value;
                }
            }
        }

        private const string RegColumnPatern = @"C;?.*X(\d*);+";
        private const string RegRowPatern = @"C;?.*Y(\d*);+";
        private const string RegGetValuePatern = @"(C;.*;)(.*)";
        private WarSylkItem lastWarItem;
        private int lastColum;

        private void ObjectiveText(List<string> body, List<WarSylkItem> list, bool isNeedUpGameData)
        {
            while (body.Count > 0)
            {
                string line = body.First();
                bool isSystemLine = true;
                body.RemoveAt(0);

                // Find ID row
                var reg = new Regex(RegRowPatern);
                var match = reg.Match(line);
                if (match.Success)
                {
                    if (lastWarItem != null) // Event msg
                        Echo(lastWarItem.ToString());

                    int id = Convert.ToInt32(match.Groups[1].Value);
                    lastWarItem = new WarSylkItem(id);
                    list.Add(lastWarItem);
                    isSystemLine = false;
                }

                if (lastWarItem != null)
                {
                    // Find Column
                    reg = new Regex(RegColumnPatern);
                    match = reg.Match(line);

                    // Get colum Index
                    if (match.Success)
                        lastColum = Convert.ToInt32(match.Groups[1].Value);

                    // Find Row Value
                    reg = new Regex(RegGetValuePatern);
                    match = reg.Match(line);
                    if (match.Success)
                    {
                        string coord = match.Groups[1].Value;
                        string value = match.Groups[2].Value;
                        lastWarItem.AddValue(lastColum, value, coord);
                        isSystemLine = false;
                    }
                    else
                    {
                        if (lastWarItem != null) // Event msg
                            Echo(lastWarItem.ToString());
                        isSystemLine = true;
                    }
                }

                if (isSystemLine && isNeedUpGameData)
                {
                    Echo(line);
                    list.Add(new WarSylkItem(line));
                }
            }
        }

        private void Echo(string msg)
        {
            EventMessanger?.Invoke(msg);
        }
    }
}
