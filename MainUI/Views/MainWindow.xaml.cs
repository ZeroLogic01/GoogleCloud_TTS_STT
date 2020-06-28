using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace MainUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SetEnvironmentVariable(object sender, RoutedEventArgs e)
        {

            var result = await DialogManager.ShowMessageAsync(this, "Are you sure?",
                $"You are about to set the user environment variable GOOGLE_APPLICATION_CREDENTIALS for the current user " +
                $"{System.Security.Principal.WindowsIdentity.GetCurrent().Name}. This change will take effect the next time you launch {Assembly.GetExecutingAssembly().GetName().Name}. The App will shutdown.",
                     MessageDialogStyle.AffirmativeAndNegative);

            System.Console.WriteLine(result);
            if (result == MessageDialogResult.Affirmative)
            {
                string programFileName = "GOOGLE_APPLICATION_CREDENTIALS_Setup.exe";
                string programPath = Path.Combine(Directory.GetCurrentDirectory(), "EnvironmentVariable", programFileName);
                if (File.Exists(programPath))
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = programPath;
                    proc.StartInfo.UseShellExecute = true;
                    //proc.StartInfo.Verb = "runas";
                    proc.Start();
                    proc.WaitForExit();
                    if (proc.ExitCode == 0)
                    {
                        Application.Current.Shutdown();
                    }
                }
            }

        }
    }
}
