using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToolModXdGui.Core;
using ToolModXdLib;
using ToolModXdLib.Models;

namespace ToolModXdGui.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ToolMod _toolMod;
        private string _lastDir;
        private string _targetName;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CellEditor> SourceList { get; set; } = new ObservableCollection<CellEditor>();
        public ObservableCollection<CellEditor> TargetList { get; set; } = new ObservableCollection<CellEditor>();

        public ICommand OpenSource { get; set; }

        public ICommand OpenTarget { get; set; }

        public ICommand SaveSource { get; set; }

        public ICommand SaveTarget { get; set; }

        public ICommand DoInject { get; set; }

        public ICommand SaveResult { get; set; }

        public string TextLog { get; set; }

        public MainViewModel()
        {
            OpenSource = new Command(SelectSourceExe);
            SaveSource = new Command(SaveSourceExe);
            OpenTarget = new Command(SelectTargetExe);
            SaveTarget = new Command(SaveTargetExe);
            DoInject = new Command(DoInjectExe);
            SaveResult = new Command(SaveResultExe);
        }

        private void OnEventMessanger(string msg)
        {
            TextLog += msg + Environment.NewLine;
        }

        private async void SelectSourceExe()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Table SYLK (*.slk)|*.slk";

            if (_lastDir == null)
                _lastDir = Environment.CurrentDirectory;

            openFileDialog.InitialDirectory = _lastDir;

            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                _lastDir = Path.GetDirectoryName(path);
                await Task.Run( () =>
                {
                    _toolMod = new ToolMod(path);
                    _toolMod.EventMessanger += OnEventMessanger;

                    _toolMod.Init();
                });

                //SourceList = new ObservableCollection<CellEditor>(_toolMod.GetCellsForSourceEditor() );
            }
        }

        private async void SaveSourceExe()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|Table SYLK (*.slk)|*.slk";
            saveFileDialog.Title = "Save an Source file";

            if (_lastDir == null)
                _lastDir = Environment.CurrentDirectory;

            saveFileDialog.InitialDirectory = _lastDir;

            if (saveFileDialog.ShowDialog() == true && saveFileDialog.FileName != "")
            {
                string path = saveFileDialog.FileName;
                _lastDir = Path.GetDirectoryName(path);

                _toolMod.SaveResult(saveFileDialog.FileName);
            }
        }

        private async void SelectTargetExe()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Table SYLK (*.slk)|*.slk";

            if (_lastDir == null)
                _lastDir = Environment.CurrentDirectory;

            openFileDialog.InitialDirectory = _lastDir;

            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                _lastDir = Path.GetDirectoryName(path);
                await Task.Run(() =>
                {
                    _toolMod.LoadTarget(path);
                });

                //TargetList = new ObservableCollection<CellEditor>(_toolMod.GetCellsForTargetEditor());
                _targetName = Path.GetFileName(path);
            }
        }

        private async void SaveTargetExe()
        {

        }

        private async void DoInjectExe()
        {
            await Task.Run(() =>
            {
                //_toolMod.LoadTarget(path);
                _toolMod.Inject();
            });
        }

        private async void SaveResultExe()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = _targetName;
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";

            saveFileDialog.InitialDirectory = _lastDir;
            if (saveFileDialog.ShowDialog() == true)
                _toolMod.SaveResult(saveFileDialog.FileName);
        }
    }
}
