using WPF.UI.ViewModels;

namespace TextEditorLauncher.UI.ViewModels
{

    public class IconViewModel : ViewModelBase
    {

        private bool _isOpened;
        private string _fileName;

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

        public string FileName
        {
            get =>
                _fileName;

            set
            {
                _fileName = value;
                NotifyPropertyChanged(nameof(FileName));
            }
        }

    }

}