using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToolModXdGui.Core;
using ToolModXdLib;

namespace ToolModXdGui.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ToolMod _toolMod;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Item> SourceList { get; set; } = new ObservableCollection<Item>() { new Item { Text = "test" } };

        public ICommand OpenSource { get; set; }

        public ICommand OpenTarget { get; set; }

        public string TextLog { get; set; }

        public MainViewModel()
        {
            OpenSource = new Command(SelectFile);

            _toolMod = new ToolMod();
            _toolMod.EventMessanger += OnEventMessanger;
        }

        private void OnEventMessanger(string msg)
        {
            TextLog += msg + Environment.NewLine;
        }

        private async void SelectFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Table SYLK (*.slk)|*.slk";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                bool valid = _toolMod.OpenFile(path);
                if (valid)
                {
                    await _toolMod.Read(path);
                    await _toolMod.Objectivation(false).ConfigureAwait(false);
                }
            }
        }
    }
}
