using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

using TextEditorLauncher.UI.Log;
using WPF.UI.Commands;
using WPF.UI.ViewModels;

namespace TextEditorLauncher.UI.ViewModels.Controls
{

    public class IconViewModel : ViewModelBase, IDisposable
    {

        private bool _isOpened;
        private string _filePath;
        private Process _executingProcess;

        private ICommand _launchTextEditorCommand;
        private ICommand _killProcessCommand;
        private ICommand _removeCommand;

        public bool IsOpened
        {
            get =>
                _isOpened;

            set
            {
                _isOpened = value;
                NotifyPropertyChanged(nameof(IsOpened));
            }
        }

        public string FilePath
        {
            get =>
                _filePath;

            set
            {
                _filePath = value;
                NotifyPropertyChanged(nameof(FilePath));
            }
        }

        public string SelectedTextEditor
        {
            private get;

            set;
        }

        public ObservableCollection<IconViewModel> Icons
        {
            private get;

            set;
        }

        public string FileName =>
            Path.GetFileName(FilePath);

        public ICommand LaunchTextEditorCommand =>
            _launchTextEditorCommand ?? (_launchTextEditorCommand = new RelayCommand(_ => LaunchTextEditor()));

        public ICommand KillProcessCommand =>
            _killProcessCommand ?? (_killProcessCommand = new RelayCommand(_ => KillProcess(), _ => IsOpened));

        public ICommand RemoveCommand =>
            _removeCommand ?? (_removeCommand = new RelayCommand(_ => Remove()));

        private void LaunchTextEditor()
        {
            if (!File.Exists(FilePath))
            {
                var message = $"File \"{FilePath}\" not exists!{Environment.NewLine}Do You want to remove it from list?";
                var severity = Severity.Error;
                if (MessageBox.Show(message, severity.ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Remove();
                }
                return;
            }
            _executingProcess = new Process()
            {
                EnableRaisingEvents = true
            };
            _executingProcess.StartInfo.FileName = SelectedTextEditor;
            _executingProcess.StartInfo.Arguments = FilePath;
            _executingProcess.Exited += (sender, eventArgs) =>
            {
                _executingProcess.Dispose();
                _executingProcess = null;
                IsOpened = false;
                Logger.Instance.Log(Severity.Notification, $"Text editor with file {FilePath} has been terminated.");
            };
            try
            {
                _executingProcess.Start();
            }
            catch (Exception)
            {
                _executingProcess.Dispose();
                _executingProcess = null;
                var severity = Severity.Error;
                var message = $"Text editor with file {FilePath} can't be started.";
                Logger.Instance.Log(severity, message);
                MessageBox.Show(message, severity.ToFriendlyString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            IsOpened = true;
            Logger.Instance.Log(Severity.Notification, $"Text editor with file {FilePath} started successfully.");
        }

        private void KillProcess()
        {
            _executingProcess?.Kill();
            IsOpened = false;
        }

        private void Remove()
        {
            Icons.Remove(this);
            Logger.Instance.Log(Severity.Notification, "Icon removed successfully.");
            Dispose();
        }

        public void Dispose()
        {
            KillProcess();
        }

    }

}