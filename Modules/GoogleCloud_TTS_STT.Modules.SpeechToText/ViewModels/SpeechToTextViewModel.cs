using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators;
using Prism.Events;
using Prism.Regions;
using System.Threading;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.ViewModels
{
    public class SpeechToTextViewModel : ViewModelBase
    {
        #region Properties

        #region Binded Properties

        private string _transcribedText;
        public string TranscribedText
        {
            get { return _transcribedText; }
            set { SetProperty(ref _transcribedText, value); }
        }

        private string _statusText = string.Empty;
        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }


        #endregion



        #region Private Properties

        private readonly IRegionManager _regionManager;
        private CancellationTokenSource _cts;

        #endregion



        #endregion

        public SpeechToTextViewModel()
        {

        }

        public SpeechToTextViewModel(IEventAggregator ea)
        {
            ea.GetEvent<StatusEvent>().Subscribe(UpdateStatusText, ThreadOption.PublisherThread, false);
        }



        #region Methods
        public void UpdateStatusText(StatusEventParameters status)
        {
            StatusText = status.Message;
        }

        #endregion

    }
}
