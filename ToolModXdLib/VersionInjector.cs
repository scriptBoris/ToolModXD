using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToolModXdLib
{
    public class VersionInjector
    {
        private List<string> sourceBody;
        private List<string> targetBody;

        public VersionInjector(string pathSource, string pathTarget)
        {
            using (var sr = new StreamReader(pathSource) )
            {
                string line;
                while( (line = sr.ReadLineAsync().Result) != null)
                {
                    sourceBody.Add(line);
                }
                sr.Close();
            }

            using (var sr = new StreamReader(pathTarget))
            {
                string line;
                while ((line = sr.ReadLineAsync().Result) != null)
                {
                    targetBody.Add(line);
                }
                sr.Close();
            }
        }

        private const string RegExPatern = "";

        public void DoObjective(List<string> body)
        {
            foreach (var line in body)
            {
                var reg = new Regex(RegHeader);
                var match = reg.Matches()
            }
        }

        public async Task Inject()
        {
            
        }
    }
}
