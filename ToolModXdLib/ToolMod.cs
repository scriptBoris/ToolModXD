using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToolModXdLib
{
    public class ToolMod
    {
        private IVersionInjector _protocol;
        private string origin;
        private string target;

        public event InjectorMsgHandler EventMessanger;

        public ToolMod(string pathFile)
        {
            origin = pathFile;
            if (Path.GetExtension(pathFile) == ".slk")
                _protocol = new VersionInjectorSlk();
            else if (Path.GetExtension(pathFile) == ".txt")
                _protocol = new VersionInjector();
            else
                _protocol = null;

            if (_protocol != null)
            {
                _protocol.EventMessanger += OnEventMessanger;
                EventMessanger?.Invoke("ToolModXd is ready\n");
                _protocol.Read(pathFile);
                _protocol.Objectivation(false);
            }
        }

        public void LoadTarget(string path)
        {
            target = path;
            EventMessanger?.Invoke($"\nStart load target: {path}");
            _protocol.LoadTarget(path);
        }

        public void Inject()
        {
            EventMessanger?.Invoke($"\nStart inject target: {origin} to -> {target}");
            _protocol.Inject();
        }

        public void SaveResult(string dirPath)
        {
            EventMessanger?.Invoke($"\nStart procedure save results: {dirPath}");
            _protocol.SaveResult(dirPath);
            EventMessanger?.Invoke($"\nCOMPLETE!");
        }

        private void OnEventMessanger(string msg)
        {
            EventMessanger?.Invoke(msg);
        }
    }
}
