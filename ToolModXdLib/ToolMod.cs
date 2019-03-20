using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolModXdLib.Core;
using ToolModXdLib.Models;

namespace ToolModXdLib
{
    public class ToolMod
    {
        private IVersionInjector _protocol;
        private string _origin;
        private string _target;

        public event InjectorMsgHandler EventMessanger;

        public ToolMod(string pathFile)
        {
            _origin = pathFile;
            if (Path.GetExtension(pathFile) == ".slk")
                _protocol = new VersionInjectorSlk();
            else if (Path.GetExtension(pathFile) == ".txt")
                _protocol = new VersionInjector();
            else
                _protocol = null;

            if (_protocol != null)
            {
                _protocol.EventMessanger += OnEventMessanger;
            }
        }

        public void Init()
        {
            EventMessanger?.Invoke($"\nStart load origin: {_origin}");
            _protocol.Read(_origin);
            _protocol.Objectivation(false);
            EventMessanger?.Invoke($"\nFinish load origin: {_origin}");

            EventMessanger?.Invoke("ToolModXd is ready\n");
        }

        public void LoadTarget(string path)
        {
            _target = path;
            EventMessanger?.Invoke($"\nStart load target: {path}");
            _protocol.LoadTarget(path);
            EventMessanger?.Invoke($"\nFinish load target: {path}");
        }

        public void Inject()
        {
            EventMessanger?.Invoke($"\nStart inject target: {_origin} to -> {_target}");
            _protocol.Inject();
            EventMessanger?.Invoke($"\nFinish inject target: {_origin} to -> {_target}");
        }

        public void SaveResult(string dirPath)
        {
            EventMessanger?.Invoke($"\nStart procedure save results: {dirPath}");
            _protocol.SaveResult(dirPath);
            EventMessanger?.Invoke($"\nCOMPLETE!");
        }

        /// <summary>
        /// Получить список данных для listFile путем обработки списка данных, полученного путем Objectivation
        /// </summary>
        /// <param name="injector">Лист в который необходимо добавить найденные данные</param>
        public void GetDataForListfile(ListFileInjector injector)
        {
            _protocol.GetDataForListfile(injector);
        }

        public void InvokeMessage(string msg)
        {
            EventMessanger?.Invoke(msg);
        }

        public List<CellEditor> GetCellsForSourceEditor()
        {
            var res = _protocol.GetCellsForSourceEditor();
            return res;
        }

        public List<CellEditor> GetCellsForTargetEditor()
        {
            return _protocol.GetCellsForTargetEditor();
        }

        private void OnEventMessanger(string msg)
        {
            EventMessanger?.Invoke(msg);
        }
    }
}
