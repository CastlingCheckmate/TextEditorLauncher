using System.Windows;

using TextEditorLauncher.UI.Log;

namespace TextEditorLauncher.UI
{

    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger.Instance.Log(Severity.Notification, "Application execution started.");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Logger.Instance
                .Log(Severity.Notification, "Application execution finished.")
                .Flush()
                .Dispose();
        }

    }

}