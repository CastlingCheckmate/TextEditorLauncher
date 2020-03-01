using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
            Icons = new ObservableCollection<IconViewModel>();
            Icons.Add(new IconViewModel() { Icons = Icons, IsOpened = false, FilePath = Path.Combine(Environment.CurrentDirectory, "a.txt") });
            Icons.Add(new IconViewModel() { Icons = Icons, IsOpened = false, FilePath = Path.Combine(Environment.CurrentDirectory, "b.txt") });
            Icons.Add(new IconViewModel() { Icons = Icons, IsOpened = false, FilePath = Path.Combine(Environment.CurrentDirectory, "c.txt") });
            SelectedTextEditor = "notepad.exe";
            Logger.Instance.Log(Severity.Notification, "Icons successfully created.");
        }

        public ObservableCollection<IconViewModel> Icons
        {
            get;

            private set;
        }

        private string SelectedTextEditor
        {
            get =>
                _selectedTextEditor;

            set
            {
                _selectedTextEditor = value;
                foreach (var icon in Icons)
                {
                    icon.SelectedTextEditor = value;
                }
                Logger.Instance.Log(Severity.Notification, $"Selected new text editor: {value}");
            }
        }

        public ICommand AddNewIconCommand =>
            _addNewIconCommand ?? (_addNewIconCommand = new RelayCommand(_ => AddNewIcon()));

        public ICommand SelectTextEditorCommand =>
            _selectTextEditorCommand ?? (_selectTextEditorCommand = new RelayCommand(_ => SelectTextEditor()));

        public ICommand AboutCommand =>
            _aboutCommand ?? (_aboutCommand = new RelayCommand(_ => About()));

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
            Icons.Add(new IconViewModel() { Icons = Icons, IsOpened = false, FilePath = openFileDialog.FileName, SelectedTextEditor = SelectedTextEditor });
            Logger.Instance.Log(Severity.Notification, "Icon added successfully.");
        }

        public void SelectTextEditor()
        {
            if (_textEditorsNames is null)
            {
                var textEditorsNames = new List<string>();
                var query = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.txt\OpenWithList");
                var valuesNames = query.GetValueNames();
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
                _textEditorsNames = textEditorsNames.ToArray();
            }
            var dialogViewModel = new TextEditorChoiceDialogViewModel(_textEditorsNames);
            var dialogView = new TextEditorChoiceDialog()
            {
                DataContext = dialogViewModel
            };
            if (!dialogView.ShowDialog().Value)
            {
                return;
            }
            SelectedTextEditor = _textEditorsNames[dialogViewModel.SelectedTextEditorIndex];
        }

        private void About()
        {
            Logger.Instance.Log(Severity.Notification, "About dialog called.");
            var message = "Developed by: Ilya Irbitskiy";
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Dispose()
        {
            foreach (var icon in Icons)
            {
                icon.Dispose();
            }
        }

    }

}