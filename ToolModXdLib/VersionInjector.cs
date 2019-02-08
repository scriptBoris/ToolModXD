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
        private List<string> _sourceBody = new List<string>();
        private List<string> _targetBody = new List<string>();

        public List<War3Object> ListSource { get; private set; } = new List<War3Object>();
        public List<War3Object> ListTarget { get; private set; } = new List<War3Object>();

        private static PropertyInfo[] _fields;

        public VersionInjector(string pathSource, string pathTarget)
        {
            _fields = typeof(War3Object).GetProperties();

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

        private const string RegHeader = @"\[....\]";

        private War3Object lastWarObj;

        public void DoObjective(List<string> body, List<War3Object> list)
        {
            for(int i = 0; i <= body.Count - 1; i++)
            {
                // Find ID
                var reg = new Regex(RegHeader);
                var match = reg.Match(body[i]);
                if (match.Success)
                {
                    lastWarObj = new War3Object(match.Value);
                    ListSource.Add(lastWarObj);
                }
                else
                {
                    if (lastWarObj == null)
                        continue;

                    // Find fields
                    var split = body[i].Split('=');
                    if (split.Length == 0)
                        continue;

                    string bodyField = split[0];
                    var matchField = _fields.FirstOrDefault(x => string.Equals(x.Name, bodyField, StringComparison.OrdinalIgnoreCase));
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
                    else
                    {
                        lastWarObj.GameBody.Add(body[i]);
                    }
                }
            }
        }

        public async Task Inject()
        {
            DoObjective(_sourceBody, ListSource);
        }
    }
}
