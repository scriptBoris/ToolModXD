using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToolModXdLib
{
    public class ToolMod : IVersionInjector
    {
        private IVersionInjector _protocol;

        public event InjectorMsgHandler EventMessanger;

        public ToolMod()
        {
            EventMessanger?.Invoke("ToolModXd is ready");
        }

        private void OnEventMessanger(string msg)
        {
            EventMessanger?.Invoke(msg);
        }

        public bool OpenFile(string pathFile)
        {
            if (Path.GetExtension(pathFile) == ".slk")
                _protocol = new VersionInjectorSlk();
            else if (Path.GetExtension(pathFile) == ".txt")
                _protocol = new VersionInjector();
            else
                _protocol = null;

            if (_protocol != null)
            {
                _protocol.EventMessanger += OnEventMessanger;
                return true;
            }

            return false;
        }

        public async Task Inject(List<object> targetList)
        {
            await _protocol.Inject(targetList);
        }

        public async Task Objectivation(bool IsLoadGameplayData)
        {
            await _protocol.Objectivation(IsLoadGameplayData);
        }

        public async Task Read(string filePath)
        {
            await _protocol.Read(filePath);
        }

        public async Task SaveResult(string dirPath)
        {
            await _protocol.SaveResult(dirPath);
        }
    }
}
