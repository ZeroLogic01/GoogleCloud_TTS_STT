using GoogleCloud_TTS_STT.Core;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Deployment;

namespace MainUI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Balconette’s Speech Services";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        public DelegateCommand EnvironmentVariableCommand { get; set; }

        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { SetProperty(ref _applicationCommands, value); }
        }

        public MainWindowViewModel() { }

        public MainWindowViewModel(IApplicationCommands applicationCommands)
        {
            var version = GetRunningVersion();
            if (version != null)
            {
                Title = $"Balconette’s Speech Services {version.Major}.{version.Minor}";

            }
            //GetRunningVersion();
            ApplicationCommands = applicationCommands;

            EnvironmentVariableCommand = new DelegateCommand(ChangeEnvironmentVariable);
        }

        private Version GetRunningVersion()
        {
            try
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
            catch
            {
                return null;
            }
        }

        private async void ChangeEnvironmentVariable()
        {
            var result = await DialogManager.ShowMessageAsync(Application.Current.MainWindow as MetroWindow, "Are you sure?",
                $"You are about to set the user environment variable GOOGLE_APPLICATION_CREDENTIALS for the current user " +
                $"{System.Security.Principal.WindowsIdentity.GetCurrent().Name}. This change will take effect the next time you launch {Assembly.GetExecutingAssembly().GetName().Name}. The App might shut down.",
                     MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative)
            {
                SetEnvironmentVariable();
            }
        }

        public static void SetEnvironmentVariable()
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

        //private async void ExecuteWindowClosingCommand()
        //{
        //    await Task.Delay(TimeSpan.FromSeconds(30));
        //}


    }
}
