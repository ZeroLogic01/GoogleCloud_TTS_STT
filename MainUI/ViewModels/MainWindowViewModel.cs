using GoogleCloud_TTS_STT.Core.EventAggregators;
using MahApps.Metro.Controls.Dialogs;
using Prism.Events;
using Prism.Mvvm;
using System.Threading.Tasks;

namespace MainUI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Google Speech Services";
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

        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(IEventAggregator ea)
        {
            ea.GetEvent<StatusTextEvent>().Subscribe(MessageReceived, ThreadOption.PublisherThread, false);
        }

        private void MessageReceived(StatusTextEventParameters status)
        {
            StatusText = status.Message;
        }

    }
}
