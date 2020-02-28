using System.Windows;

using TextEditorLauncher.UI.ViewModels.Windows;

namespace TextEditorLauncher.UI.Views.Windows
{

    public partial class MainView : Window
    {

        public MainView()
        {
            InitializeComponent();
            Closed += (sender, eventArgs) =>
            {
                ((MainViewModel)DataContext)?.Dispose();
            };
        }

    }

}