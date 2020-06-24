using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Helpers;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Models;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Views;
using Prism.Commands;
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

      

        //public TranscriptionSettingsViewModel TranscriptionSettingsVM { get; set; }

        #endregion

        #region Private Properties

        private readonly IRegionManager _regionManager;
        private CancellationTokenSource _cts;

        #endregion



        #endregion

        public SpeechToTextViewModel()
        {

        }

        public SpeechToTextViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _cts = new CancellationTokenSource();

            //TranscriptionSettingsVM = new TranscriptionSettingsViewModel();

           
        }



    }
}
