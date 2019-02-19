using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolModXdLib.Models
{
    internal class WarGameItem : IData
    {
        /// <summary>
        /// Идентификатор формата [A09F]
        /// </summary>
        public string Id { get; set; }

        public string Art { get; set; }
        public string Unart { get; set; }
        public string ResearchArt { get; set; }
        public string MissileArt { get; set; }
        public string LightningEffect { get; set; }
        public string BuffArt { get; set; }
        public string AreaEffectArt { get; set; }
        public string TargetArt { get; set; }
        public string TargetAttach { get; set; }
        public string TargetAttach1 { get; set; }
        public string TargetAttach2 { get; set; }
        public string TargetAttach3 { get; set; }
        public string TargetAttach4 { get; set; }
        public string TargetAttach5 { get; set; }
        public string TargetAttachCount { get; set; }
        public string CasterArt { get; set; }
        public string CasterAttach { get; set; }
        public string CasterAattach1 { get; set; }
        public string CasterAattach2 { get; set; }
        public string CasterAattach3 { get; set; }
        public string CasterAattach4 { get; set; }
        public string CasterAattach5 { get; set; }
        public string CasterAttachCount { get; set; }
        public string EffectArt { get; set; }
        public string EffectSound { get; set; }
        public string SpecialArt{ get; set; }
        public string SpecialAttach { get; set; }
        public string AnimNames { get; set; }
        public List<string> GameBody { get; set; } = new List<string>();

        public WarGameItem(string id)
        {
            Id = id;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            string res = "";
            foreach(var prop in VersionInjector.PropsWar3Obj)
            {
                if (prop.Name == nameof(Id))
                {
                    res += (string) prop.GetValue(this) + "\n";
                }
                else if (prop.Name == nameof(GameBody))
                {
                    foreach (var item in GameBody)
                        res += item + "\n";
                }
                else
                {
                    string propValue = (string) prop.GetValue(this);
                    if (propValue == null)
                        continue;

                    res += $"{prop.Name}={propValue}\n";
                }
            }
            if (res == "")
                return "none";
            return res;
        }

        public CellEditor GetCellEditor()
        {
            var res = new CellEditor
            {
                Header = Id,
                Source = this
            };

            foreach (var prop in VersionInjector.PropsWar3Obj)
            {
                string propName = prop.Name;
                if (propName != nameof(Id) && propName != nameof(GameBody))
                {
                    string propValue = (string)prop.GetValue(this);
                    if (propValue == null)
                        continue;

                    var cell = new CellData()
                    {
                        Key = propName,
                        Value = propValue,
                        Data = this,
                    };

                    res.Datas.Add(cell);
                }
            }

            return res;
        }

        public void Refresh(string propname, string newValue)
        {
            foreach (var prop in VersionInjector.PropsWar3Obj)
            {
                if (prop.Name == propname)
                {
                    prop.SetValue(this, newValue);
                    return;
                }
            }
        }
    }
}
