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

        // запущен ли текстовый редактор с текущим файлом
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

        // полный путь к файлу
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

        // имя текстового редактора, которым открываются файлы
        public string SelectedTextEditor
        {
            private get;

            set;
        }

        // коллекция вьюмоделей иконок
        public ObservableCollection<IconViewModel> Icons
        {
            private get;

            set;
        }

        // имя файла (без полного пути)
        public string FileName =>
            Path.GetFileName(FilePath);

        // команда запуска текстового редактора с текущим файлов
        // невозможность открыть через лончер один и тот же файл дважды обеспечивается засчёт 
        public ICommand LaunchTextEditorCommand =>
            _launchTextEditorCommand ?? (_launchTextEditorCommand = new RelayCommand(_ => LaunchTextEditor()));

        // команда удаления запущенного процесса
        public ICommand KillProcessCommand =>
            _killProcessCommand ?? (_killProcessCommand = new RelayCommand(_ => KillProcess(), _ => IsOpened));

        // команда удаления текущей иконки из списка иконок
        public ICommand RemoveCommand =>
            _removeCommand ?? (_removeCommand = new RelayCommand(_ => Remove()));

        private void LaunchTextEditor()
        {
            // если файла не существует, предлагаем юзеру удалить иконку из списка
            if (!File.Exists(FilePath))
            {
                var severity = Severity.Error;
                Logger.Instance.Log(severity, $"File \"{FilePath}\" can't be opened because it is not exists!");
                var message = $"File \"{FilePath}\" not exists!{Environment.NewLine}Do You want to remove it from list?";
                if (MessageBox.Show(message, severity.ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Remove();
                }
                return;
            }
            // создаём объект, связанный с процессом запущенного текстового редактора с текущим файликом
            _executingProcess = new Process()
            {
                EnableRaisingEvents = true
            };
            _executingProcess.StartInfo.FileName = SelectedTextEditor;
            _executingProcess.StartInfo.Arguments = FilePath;
            // при завершении процесса, обновляем UI и пишем лог
            _executingProcess.Exited += (sender, eventArgs) =>
            {
                _executingProcess.Dispose();
                _executingProcess = null;
                IsOpened = false;
                Logger.Instance.Log(Severity.Notification, $"Text editor with file {FilePath} has been terminated.");
            };
            try
            {
                // пытаемся запустить процесс, если что-то идёт не так, пишем лог, освобождаем ресурсы объекта Process и сообщаем пользователю об ошибке запуска
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
            // всё открылось, пишем лог и обновляем состояния кнопок на UI
            IsOpened = true;
            Logger.Instance.Log(Severity.Notification, $"Text editor with file {FilePath} started successfully.");
        }

        private void KillProcess()
        {
            // убиваем процесс и обновляем состояния кнопок на UI
            _executingProcess?.Kill();
            IsOpened = false;
        }

        private void Remove()
        {
            // удаляем текущую вьюмодель из observablecollection и пишем лог
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