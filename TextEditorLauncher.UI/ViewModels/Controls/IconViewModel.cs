using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

using TextEditorLauncher.UI.Log;
using WPF.UI.Commands;
using WPF.UI.ViewModels;

namespace TextEditorLauncher.UI.ViewModels.Controls
{

    public class IconViewModel : ViewModelBase, IDisposable
    {

        private const string NotepadEditor = "notepad.exe";

        private bool _isOpened;
        private string _filePath;
        private Process _executingProcess;

        private ICommand _launchTextEditorCommand;
        private ICommand _killProcessCommand;

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

        public string FileName =>
            Path.GetFileName(FilePath);

        public ICommand LaunchTextEditorCommand =>
            _launchTextEditorCommand ?? (_launchTextEditorCommand = new RelayCommand(_ => LaunchTextEditor()));

        public ICommand KillProcessCommand =>
            _killProcessCommand ?? (_killProcessCommand = new RelayCommand(_ => KillProcess(), _ => IsOpened));

        private void LaunchTextEditor()
        {
            if (!File.Exists(FilePath))
            {
                File.Create(FilePath);
            }
            _executingProcess = new Process()
            {
                EnableRaisingEvents = true,
            };
            _executingProcess.StartInfo.FileName = NotepadEditor;
            _executingProcess.StartInfo.Arguments = FilePath;
            _executingProcess.Exited += (sender, eventArgs) =>
            {
                _executingProcess.Dispose();
                _executingProcess = null;
                IsOpened = false;
                Logger.Instance.Log(Severity.Notification, $"Text editor with file {FilePath} has been terminated.");
            };
            IsOpened = true;
            Logger.Instance.Log(Severity.Notification, $"Text editor with file {FilePath} started successfully.");
            _executingProcess.Start();
        }

        private void KillProcess()
        {
            _executingProcess?.Kill();
            IsOpened = false;
        }

        public void Dispose()
        {
            KillProcess();
        }

    }

}