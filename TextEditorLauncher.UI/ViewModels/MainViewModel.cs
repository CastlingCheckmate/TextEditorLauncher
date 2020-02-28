using System.Collections.ObjectModel;

using WPF.UI.ViewModels;

namespace TextEditorLauncher.UI.ViewModels
{

    public class MainViewModel : ViewModelBase
    {

        public MainViewModel()
        {
            Icons = new ObservableCollection<IconViewModel>();
            Icons.Add(new IconViewModel() { IsOpened = false, FileName = "a.txt" });
            Icons.Add(new IconViewModel() { IsOpened = false, FileName = "b.txt" });
            Icons.Add(new IconViewModel() { IsOpened = false, FileName = "c.txt" });
        }

        public ObservableCollection<IconViewModel> Icons
        {
            get;

            private set;
        }

    }

}