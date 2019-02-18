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

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CellEditor> SourceList { get; set; } = new ObservableCollection<CellEditor>();

        public ICommand OpenSource { get; set; }

        public ICommand OpenTarget { get; set; }

        public ICommand SaveSource { get; set; }

        public ICommand SaveTarget { get; set; }

        public string TextLog { get; set; }

        public MainViewModel()
        {
            OpenSource = new Command(SelectFile);
            SaveSource = new Command(SaveSourceExe);
        }

        private void OnEventMessanger(string msg)
        {
            TextLog += msg + Environment.NewLine;
        }

        private async void SelectFile()
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

                SourceList = new ObservableCollection<CellEditor>(_toolMod.GetCellsEditor() );
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
    }
}
