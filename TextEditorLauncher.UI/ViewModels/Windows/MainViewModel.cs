using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

using TextEditorLauncher.UI.ViewModels.Controls;
using WPF.UI.Commands;
using WPF.UI.ViewModels;

namespace TextEditorLauncher.UI.ViewModels.Windows
{

    public class MainViewModel : ViewModelBase, IDisposable
    {

        private ICommand _addNewIconCommand;

        public MainViewModel()
        {
            Icons = new ObservableCollection<IconViewModel>();
            Icons.Add(new IconViewModel() { IsOpened = false, FilePath = Path.Combine(Environment.CurrentDirectory, "a.txt") });
            Icons.Add(new IconViewModel() { IsOpened = false, FilePath = Path.Combine(Environment.CurrentDirectory, "b.txt") });
            Icons.Add(new IconViewModel() { IsOpened = false, FilePath = Path.Combine(Environment.CurrentDirectory, "c.txt") });
        }

        public ObservableCollection<IconViewModel> Icons
        {
            get;

            private set;
        }

        public ICommand AddNewIconCommand =>
            _addNewIconCommand ?? (_addNewIconCommand = new RelayCommand(_ => AddNewIcon()));

        private void AddNewIcon()
        {
            var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                CheckFileExists = true,
                Title = "New icon file...",
                Filter = "Text file (*.txt)|*.txt"
            };
            if (!openFileDialog.ShowDialog().Value)
            {
                return;
            }
            Icons.Add(new IconViewModel() { IsOpened = false, FilePath = openFileDialog.FileName });
        }

        public void Dispose()
        {
            for (var i = 0; i < Icons.Count; i++)
            {
                Icons[i].Dispose();
            }
        }

    }

}