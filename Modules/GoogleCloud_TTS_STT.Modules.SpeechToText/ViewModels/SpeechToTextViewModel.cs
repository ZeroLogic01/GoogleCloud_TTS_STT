using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Helpers;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Models;
using Prism.Commands;
using Prism.Mvvm;
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

        private bool _isSpeechToTextButtonEnabled = true;
        public bool IsSpeechToTextButtonEnabled
        {
            get { return _isSpeechToTextButtonEnabled; }
            set
            {
                SetProperty(ref _isSpeechToTextButtonEnabled, value);

                IsCancelButtonEnabled = !_isSpeechToTextButtonEnabled;
                IsCancelButtonVisisble = _isSpeechToTextButtonEnabled ? Visibility.Collapsed : Visibility.Visible;
                CancelSpeechToTextCommand.RaiseCanExecuteChanged();
            }
        }

        #region Cancel Speech To Text Button

        private bool _isCancelButtonEnabled = false;
        public bool IsCancelButtonEnabled
        {
            get { return _isCancelButtonEnabled; }
            set
            {
                SetProperty(ref _isCancelButtonEnabled, value);
            }
        }

        private Visibility _isCancelButtonVisisble = Visibility.Collapsed;
        public Visibility IsCancelButtonVisisble
        {
            get { return _isCancelButtonVisisble; }
            set { SetProperty(ref _isCancelButtonVisisble, value); }
        }

        #endregion


        #region Supported Languages List

        private List<LanguageModel> _supportedLangauges = LanguageHelper.GetSupportedLangaugesList();
        public List<LanguageModel> SupportedLangauges
        {
            get { return _supportedLangauges; }
            set { SetProperty(ref _supportedLangauges, value); }
        }

        private LanguageModel _selectedLangauge;
        public LanguageModel SelectedLangauge
        {
            get { return _selectedLangauge; }
            set { SetProperty(ref _selectedLangauge, value); }
        }

        #endregion

        #region Supported Transcription Models

        private List<TranscriptionModelModel> _supportedTranscriptionModels = ModuleHelper.GetAllTranscriptionModels();
        public List<TranscriptionModelModel> SupportedTranscriptionModels
        {
            get { return _supportedTranscriptionModels; }
            set { SetProperty(ref _supportedTranscriptionModels, value); }
        }

        private TranscriptionModelModel _selectedTranscriptionModel;
        public TranscriptionModelModel SelectedTranscriptionModel
        {
            get { return _selectedTranscriptionModel; }
            set { SetProperty(ref _selectedTranscriptionModel, value); }
        }


        #endregion


        #region Speaker Diarization

        private List<SpeakerDiarization> _speakerDiarizationList = ModuleHelper.GetSpeakerDiarizationList();
        public List<SpeakerDiarization> SpeakerDiarizationList
        {
            get { return _speakerDiarizationList; }
            set { SetProperty(ref _speakerDiarizationList, value); }
        }

        private SpeakerDiarization _selectedSpeakerDiarization;
        public SpeakerDiarization SelectedSpeakerDiarization
        {
            get { return _selectedSpeakerDiarization; }
            set
            {
                SetProperty(ref _selectedSpeakerDiarization, value);

                // let the user select speaker count only if the SelectedSpeakerDiarization.Value is not off neither Recognize1SpeakerPerChannel
                IsSpeakersComboboxEnabled = value.Value != SpeakerDiarizationEnum.Off
                                            && value.Value != SpeakerDiarizationEnum.Recognize1SpeakerPerChannel;
            }
        }



        #endregion

        #region Speakers

        private List<Speaker> _speakersList = ModuleHelper.GetSpeakersList();
        public List<Speaker> SpeakersList
        {
            get { return _speakersList; }
            set { SetProperty(ref _speakersList, value); }
        }

        private Speaker _selectedSpeakerCount;
        public Speaker SelectedSpeakerCount
        {
            get { return _selectedSpeakerCount; }
            set { SetProperty(ref _selectedSpeakerCount, value); }
        }

        private bool _isSpeakersComboboxEnabled;
        public bool IsSpeakersComboboxEnabled
        {
            get { return _isSpeakersComboboxEnabled; }
            set { SetProperty(ref _isSpeakersComboboxEnabled, value); }
        }

        #endregion




        #endregion

        #region General Properties

        private CancellationTokenSource _cts;


        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// The command to control speech to text
        /// </summary>
        public DelegateCommand SpeechToTextCommand { get; set; }

        /// <summary>
        /// The command to stop (cancel) speech to text
        /// </summary>
        public DelegateCommand CancelSpeechToTextCommand { get; set; }


        #endregion

        public SpeechToTextViewModel()
        {
            SelectedLangauge = SupportedLangauges.FirstOrDefault(lang => lang.LanguageCode.Equals("en"));
            SelectedTranscriptionModel = SupportedTranscriptionModels.FirstOrDefault(model => model.ModelName.Equals("Default", StringComparison.OrdinalIgnoreCase));
            SelectedSpeakerDiarization = SpeakerDiarizationList.FirstOrDefault(x => x.Value == SpeakerDiarizationEnum.Off);
            SelectedSpeakerCount = SpeakersList.FirstOrDefault(x => x.Count == 2);

            _cts = new CancellationTokenSource();

            SpeechToTextCommand = new DelegateCommand(PerformSpeechToText, CanPerformSpeechToText);
            CancelSpeechToTextCommand = new DelegateCommand(CancelSpeechToText, CanCancelSpeechToText);


        }


        #region Methods

        #region STT Command Methods

        private async void PerformSpeechToText()
        {
            IsSpeechToTextButtonEnabled = false;
            SpeechToTextCommand.RaiseCanExecuteChanged();

            try
            {
                Console.WriteLine("Performing Speech to Text (Message)");
                await Task.Delay(10000, _cts.Token);
            }
            catch (TaskCanceledException) { }
            catch (Exception e)
            {
                await AppHelper.ShowMessage("Error", ExceptionHelper.ExtractExceptionMessage(e));
            }

            IsSpeechToTextButtonEnabled = true;
            SpeechToTextCommand.RaiseCanExecuteChanged();
        }

        private bool CanPerformSpeechToText()
        {
            return IsSpeechToTextButtonEnabled;
        }

        #endregion

        #region Cancel Speech To Text Command Methods


        private async void CancelSpeechToText()
        {
            try
            {
                IsCancelButtonEnabled = false;
                CancelSpeechToTextCommand.RaiseCanExecuteChanged();


                if (!_cts.IsCancellationRequested)
                {
                    _cts.Cancel(true);
                }
            }
            catch (Exception) { }
            finally
            {
                _cts?.Dispose(); // Clean up old token source....
                _cts = new CancellationTokenSource(); // "Reset" the cancellation token source...
               
                IsCancelButtonVisisble = Visibility.Collapsed;
            }
        }
        private bool CanCancelSpeechToText()
        {
            return !IsSpeechToTextButtonEnabled;
        }

        #endregion

        #endregion



    }
}
