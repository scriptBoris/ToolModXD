using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolModXdLib.Models
{
    public class War3Object
    {
        /// <summary>
        /// Идентификатор формата [A09F]
        /// </summary>
        public string Id { get; set; }
        public string Art { get; set; }
        public string Unart { get; set; }
        public string ResearchArt { get; set; }
        public string MissileArt { get; set; }
        public string BuffArt { get; set; }
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
        public string EffectArt { get; set; }
        public string SpecialArt{ get; set; }
        public string AnimNames { get; set; }
        public List<string> GameBody { get; set; } = new List<string>();

        public War3Object(string id)
        {
            Id = id;
        }

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
    }
}
