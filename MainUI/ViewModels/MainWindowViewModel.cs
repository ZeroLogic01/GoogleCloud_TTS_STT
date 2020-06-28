using GoogleCloud_TTS_STT.Core;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

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



        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { SetProperty(ref _applicationCommands, value); }
        }

        public MainWindowViewModel() { }

        public MainWindowViewModel(IApplicationCommands applicationCommands)
        {
            // WindowClosingCommand = new DelegateCommand(ExecuteWindowClosingCommand);
            //applicationCommands.WindowClosingCommand.RegisterCommand(WindowClosingCommand);
            ApplicationCommands = applicationCommands;
        }

        //private async void ExecuteWindowClosingCommand()
        //{
        //    await Task.Delay(TimeSpan.FromSeconds(30));
        //}


    }
}
