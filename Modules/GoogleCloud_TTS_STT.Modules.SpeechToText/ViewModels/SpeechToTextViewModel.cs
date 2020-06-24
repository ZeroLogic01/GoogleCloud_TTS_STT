using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Helpers;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Models;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.ViewModels
{
    public class SpeechToTextViewModel : BindableBase
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
