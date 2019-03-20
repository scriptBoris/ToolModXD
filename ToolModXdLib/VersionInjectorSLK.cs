using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToolModXdLib.Core;
using ToolModXdLib.Models;

namespace ToolModXdLib
{
    internal class VersionInjectorSlk : IVersionInjector
    {
        private List<string> _sourceBody = new List<string>();
        private List<WarSylkItem> _listTarget = new List<WarSylkItem>();
        private List<WarSylkItem> _listSource = new List<WarSylkItem>();

        public event InjectorMsgHandler EventMessanger;

        public VersionInjectorSlk()
        {
        }

        public void Read(string pathSource)
        {
            using (var sr = new StreamReader(pathSource))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    _sourceBody.Add(line);
                }
                sr.Close();
            }
            Echo($"End read. Count: {_sourceBody.Count}");
        }

        public void Objectivation(bool isLoadGameplayData)
        {
            ObjectiveText(_sourceBody, _listSource, isLoadGameplayData);
        }

        public void LoadTarget(string filePath)
        {
            var targetList = new List<string>();
            Echo($"Read target file: {filePath}");
            using (var sr = new StreamReader(filePath))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    targetList.Add(line);
                }
                sr.Close();
            }

            Echo($"Start procedure objectivation {filePath}");
            ObjectiveText(targetList, _listTarget, true);
        }

        public void Inject()
        {
            while (_listSource.Count > 0)
            {
                var source = _listSource.First();
                _listSource.RemoveAt(0);

                if (source.NumberRow == 1)
                    continue;

                if (source.SystemLine == null)
                {
                    var target = _listTarget.FirstOrDefault(x => x.RawCode == source.RawCode);
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
                foreach (var target in _listTarget)
                {
                    sw.WriteLine(target.ToString());
                }
            }

            Echo("Result is saved in: " + path);
        }

        private void Import(WarSylkItem from, WarSylkItem target)
        {
            bool isChangedTargetData = false;
            foreach(var fromItem in from.Columns)
            {
                // filter
                switch (fromItem.ColumnId)
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

                var targetItem = target.Columns.FirstOrDefault(x => x.ColumnId == fromItem.ColumnId);
                if (targetItem != null && fromItem.Value != targetItem.Value)
                {
                    Echo($"{target.RawCode} get import {fromItem.Value}, old: {targetItem.Value}");
                    targetItem.Value = fromItem.Value;
                }
                else if (targetItem == null)
                {
                    Echo($"{target.RawCode} new value: {fromItem.Value}");
                    target.Columns.Add(new WarSylkColumn(fromItem.ColumnId, fromItem.Value, fromItem.Coordinate) );
                    isChangedTargetData = true;
                }
            }

            if (isChangedTargetData)
                target.Columns.OrderBy(x => x.ColumnId);
        }

        private const string RegColumnPatern = @"C;?.*X(\d*);+";
        private const string RegRowPatern = @"C;?.*Y(\d*);+";
        private const string RegGetValuePatern = @"(C;.*;)(.*)";
        private WarSylkItem lastWarItem;
        private int lastColum;

        private void ObjectiveText(List<string> body, List<WarSylkItem> list, bool isNeedUpGameData)
        {
            int i = 0;
            int max = body.Count - 1;
            while (i <= max)
            {
                string line = body[i];
                bool isSystemLine = true;
                //string line = body.First();
                //if (line == null)
                //    break;

                // Find ID row
                var reg = new Regex(RegRowPatern);
                var match = reg.Match(line);
                if (match.Success)
                {
                    if (lastWarItem != null) // Event msg
                        Echo(lastWarItem.ToStringConsole());

                    int rowNumber = Convert.ToInt32(match.Groups[1].Value);
                    lastWarItem = new WarSylkItem(i+1);
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
                            Echo(lastWarItem.ToStringConsole());
                        isSystemLine = true;
                    }
                }

                if (isSystemLine && isNeedUpGameData)
                {
                    list.Add(new WarSylkItem(i+1, line));
                }

                i++;
            }

            body.Clear();
            Console.WriteLine("EndLoop");
        }

        private void Echo(string msg)
        {
            EventMessanger?.Invoke(msg);
        }

        public void GetDataForListfile(ListFileInjector injector)
        {
            bool isFirst = true;
            foreach (var item in _listSource)
            {
                if (!isFirst)
                    injector.Add(item.ModelPath);

                isFirst = false;
            }
        }

        public List<CellEditor> GetCellsForSourceEditor()
        {
            return null;
            var res = new List<CellEditor>();
            foreach (var item in _listSource)
            {
                res.Add(item.GetCellEditor() );
            }

            return res;
        }

        public List<CellEditor> GetCellsForTargetEditor()
        {
            return null;

            var res = new List<CellEditor>();
            foreach (var item in _listTarget)
            {
                res.Add(item.GetCellEditor());
            }

            return res;
        }
    }
}
