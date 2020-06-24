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
using System.Threading.Tasks;
using System.Windows;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.ViewModels
{
    public class TranscriptionSettingsViewModel : BindableBase
    {
        #region Binded Properties

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

        #region Choose File button

        private bool _isChooseFileButtonEnabled = true;
        public bool IsChooseFileButtonEnabled
        {
            get { return _isChooseFileButtonEnabled; }
            set
            {
                SetProperty(ref _isChooseFileButtonEnabled, value);
            }
        }

        #endregion

        #endregion

        #region Private Properties

        private readonly IRegionManager _regionManager;

        #endregion

        #region Commands

        public DelegateCommand ChooseFileCommand { get; set; }

        #endregion

        #region Constructor
       
        public TranscriptionSettingsViewModel()
        {

        }

        public TranscriptionSettingsViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            SelectedLangauge = SupportedLangauges.FirstOrDefault(lang => lang.LanguageCode.Equals("en"));
            SelectedTranscriptionModel = SupportedTranscriptionModels.FirstOrDefault(model => model.ModelName.Equals("Default", StringComparison.OrdinalIgnoreCase));
            SelectedSpeakerDiarization = SpeakerDiarizationList.FirstOrDefault(x => x.Value == SpeakerDiarizationEnum.Off);
            SelectedSpeakerCount = SpeakersList.FirstOrDefault(x => x.Count == 2);

            ChooseFileCommand = new DelegateCommand(ActivateFileSelectionView).ObservesProperty(() => IsChooseFileButtonEnabled);


        }

        private void ActivateFileSelectionView()
        {
            IsChooseFileButtonEnabled = false;
            _regionManager.RequestNavigate(Core.Regions.RegionNames.SpeechToTextAPIConfigRegion, nameof(SourceFileView));
            IsChooseFileButtonEnabled = true;
        }

        #endregion
    }
}
