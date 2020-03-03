using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using TextEditorLauncher.UI.Log;
using TextEditorLauncher.UI.ViewModels.Controls;
using TextEditorLauncher.UI.Views.Windows;
using WPF.UI.Commands;
using WPF.UI.ViewModels;

namespace TextEditorLauncher.UI.ViewModels.Windows
{

    public sealed class MainViewModel : ViewModelBase, IDisposable
    {

        private string _selectedTextEditor;
        private string[] _textEditorsNames;
        private ICommand _addNewIconCommand;
        private ICommand _selectTextEditorCommand;
        private ICommand _aboutCommand;

        public MainViewModel()
        {
            // создаём коллекцию, кладём туда файлики a.txt, b.txt, c.txt (по тз)
            Icons = new ObservableCollection<IconViewModel>();
            Icons.Add(new IconViewModel() { Icons = Icons, IsOpened = false, FilePath = Path.Combine(Environment.CurrentDirectory, "a.txt") });
            Icons.Add(new IconViewModel() { Icons = Icons, IsOpened = false, FilePath = Path.Combine(Environment.CurrentDirectory, "b.txt") });
            Icons.Add(new IconViewModel() { Icons = Icons, IsOpened = false, FilePath = Path.Combine(Environment.CurrentDirectory, "c.txt") });
            // изначально открываем файлы через стандартный блокнот
            SelectedTextEditor = "notepad.exe";
            // логи
            Logger.Instance.Log(Severity.Notification, "Icons successfully created.");
        }

        // коллекция вьюмоделей иконок
        public ObservableCollection<IconViewModel> Icons
        {
            get;

            private set;
        }

        // текущий текстовый редактор которым открываются файлы
        private string SelectedTextEditor
        {
            get =>
                _selectedTextEditor;

            set
            {
                _selectedTextEditor = value;
                // сообщаем иконкам о том, что текстовый редактор изменился
                foreach (var icon in Icons)
                {
                    icon.SelectedTextEditor = value;
                }
                Logger.Instance.Log(Severity.Notification, $"Selected new text editor: {value}");
            }
        }

        // команда добавления новой иконки
        public ICommand AddNewIconCommand =>
            _addNewIconCommand ?? (_addNewIconCommand = new RelayCommand(_ => AddNewIcon()));

        // команда выбора другого текстового редактора
        public ICommand SelectTextEditorCommand =>
            _selectTextEditorCommand ?? (_selectTextEditorCommand = new RelayCommand(_ => SelectTextEditor()));

        // команда для about messagebox 
        public ICommand AboutCommand =>
            _aboutCommand ?? (_aboutCommand = new RelayCommand(_ => About()));

        private void AddNewIcon()
        {
            // диалог выбора файла - начинаем выбор с директории exe-шника, файл на момент создания иконки должен существовать, фильтр - .txt (текстовые файлы)
            var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                CheckFileExists = true,
                Title = "New icon file...",
                Filter = "Text file (*.txt)|*.txt"
            };
            // если не была нажата кнопочка "OK" в диалоге, выходим
            if (!openFileDialog.ShowDialog().Value)
            {
                return;
            }
            // если такой же путь к файлу уже задан в другой иконке, сообщаем об этом юзеру, пишем лог и выходим
            if (Icons.Any(icon => icon.FilePath.Equals(openFileDialog.FileName)))
            {
                var severity = Severity.Warning;
                var message = "File is already in icons list!";
                Logger.Instance.Log(severity, message);
                MessageBox.Show(message, severity.ToFriendlyString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // создаём новую иконку и добавляем её в коллекцию вьюмоделей, пишем лог
            Icons.Add(new IconViewModel() { Icons = Icons, IsOpened = false, FilePath = openFileDialog.FileName, SelectedTextEditor = SelectedTextEditor });
            Logger.Instance.Log(Severity.Notification, "Icon added successfully.");
        }

        public void SelectTextEditor()
        {
            // при первом запросе на выбор текстового редактора
            if (_textEditorsNames is null)
            {
                var textEditorsNames = new List<string>();
                // лезем в реестр и смотрим, какими приложениями открывались файлы с расширением .txt
                // это единственный способ который я нашёл, чтобы сделать что-то похожее на то, что требуется
                var query = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.txt\OpenWithList");
                var valuesNames = query.GetValueNames();
                // тут похоже на костыли
                // пытаемся через каждый полученный exe-шник для текстового редактора открыть лог-файл
                // если открыли, значит ок, добавляем в список текущее имя exe-шника, если нет, идём дальше
                foreach (var valueName in valuesNames)
                {
                    var maybeTextEditorName = (string)query.GetValue(valueName);
                    var process = default(Process);
                    try
                    {
                        process = Process.Start(maybeTextEditorName, Path.Combine(Environment.CurrentDirectory, Logger.LogFileName));
                    }
                    catch (FileNotFoundException)
                    {
                        continue;
                    }
                    catch (Win32Exception)
                    {
                        continue;
                    }
                    finally
                    {
                        process?.Kill();
                        process?.Dispose();
                    }
                    textEditorsNames.Add(maybeTextEditorName);
                }
                // получаем массив строк с именами текстовых редакторов и сохраняем его во вьюмодели главного окна
                _textEditorsNames = textEditorsNames.ToArray();
            }
            // передаём массив строк с именами редакторов во вьюмодель окна для выбора текстового редактора
            var dialogViewModel = new TextEditorChoiceDialogViewModel(_textEditorsNames);
            // создаём диалог и задаём ему DataContext созданной вьюмоделью
            var dialogView = new TextEditorChoiceDialog()
            {
                DataContext = dialogViewModel,
                DialogResult = false
            };
            // при нажатии кнопки Cancel или закрытии окна через крестик, ничего не делаем
            if (!dialogView.ShowDialog().Value)
            {
                return;
            }
            // устанавливаем текущий редактор и сообщаем об этом viewmodel'ям иконок
            SelectedTextEditor = _textEditorsNames[dialogViewModel.SelectedTextEditorIndex];
        }

        // отображаем aboutbox
        private void About()
        {
            Logger.Instance.Log(Severity.Notification, "About dialog called.");
            var message = "Developed by: Ilya Irbitskiy";
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // чистим все ресурсы связанные с созданными иконками (убиваем связанные с иконками запущенные процессы текстовых редакторов)
        public void Dispose()
        {
            foreach (var icon in Icons)
            {
                icon.Dispose();
            }
        }

    }

}