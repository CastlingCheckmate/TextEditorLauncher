using System.Windows;

using TextEditorLauncher.UI.ViewModels.Windows;

namespace TextEditorLauncher.UI.Views.Windows
{

    public partial class MainView : Window
    {

        public MainView()
        {
            InitializeComponent();
            // при закрытии окна освобождаем ViewModel
            // раньше было падение из-за того, что сначала закрывался StreamWriter, связанный с логгером,
            // после чего Dispos'илась ViewModel, и если какой-либо редактор открыт, он закрывался, и при этом была попытка записи в лог-файл,
            // однако StreamWriter, через который производилась попытка записи лога, уже закрыт
            // поэтому вместо события Closed (после закрытия окна) теперь используется Closing (при закрытии окна)
            Closing += (sender, eventArgs) =>
            {
                ((MainViewModel)DataContext)?.Dispose();
            };
        }

    }

}