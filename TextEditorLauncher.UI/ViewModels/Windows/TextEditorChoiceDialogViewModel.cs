using System.Windows;
using System.Windows.Input;

using WPF.UI.Commands;
using WPF.UI.ViewModels;

namespace TextEditorLauncher.UI.ViewModels.Windows
{

    public class TextEditorChoiceDialogViewModel : ViewModelBase
    {

        private string[] _textEditors;
        private int _selectedTextEditorIndex;

        private ICommand _acceptCommand;
        private ICommand _cancelCommand;

        public TextEditorChoiceDialogViewModel(string[] textEditors)
        {
            // сохраняем массив имён текстовых редакторов из которых юзер может выбрать редактор
            TextEditors = textEditors;
        }

        // массив имён текстовых редакторов
        public string[] TextEditors
        {
            get =>
                _textEditors;

            private set
            {
                _textEditors = value;
                NotifyPropertyChanged(nameof(TextEditors));
            }
        }

        // индекс текущего выбранного имени текстового редактора из массива
        public int SelectedTextEditorIndex
        {
            get =>
                _selectedTextEditorIndex;

            set
            {
                _selectedTextEditorIndex = value;
                NotifyPropertyChanged(nameof(SelectedTextEditorIndex));
            }
        }

        // у двух команд ниже ссылка на вьюху диалога приезжает из параметров (ищем для кнопки содержащий её контейнер типа Window)

        // команда подтверждения выбора редактора
        public ICommand AcceptCommand =>
            _acceptCommand ?? (_acceptCommand = new RelayCommand(dialogView => Accept((Window)dialogView)));

        // команда отмены выбора редактора
        public ICommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new RelayCommand(dialogView => Cancel((Window)dialogView)));

        private void Accept(Window dialogView)
        {
            // говорим о том что выбор был подтверждён пользователем и завершаем диалог
            dialogView.DialogResult = true;
            dialogView.Close();
        }

        private void Cancel(Window dialogView)
        {
            // говорим о том что выбор не был сделан и завершаем диалог
            dialogView.DialogResult = false;
            dialogView.Close();
        }

    }

}