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
            TextEditors = textEditors;
        }

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

        public ICommand AcceptCommand =>
            _acceptCommand ?? (_acceptCommand = new RelayCommand(dialogView => Accept((Window)dialogView)));

        public ICommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new RelayCommand(dialogView => Cancel((Window)dialogView)));

        private void Accept(Window dialogView)
        {
            dialogView.DialogResult = true;
            dialogView.Close();
        }

        private void Cancel(Window dialogView)
        {
            dialogView.DialogResult = false;
            dialogView.Close();
        }

    }

}