using Google.Protobuf.WellKnownTypes;
using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Interfaces;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Models;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Enums;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Helpers;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.ViewModels
{
    public class SpeechToTextViewModel : ViewModelBase
    {
        #region Properties

        #region Binded Properties

        private string _statusText = string.Empty;
        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        private bool _isAutomaticPunctuationEnabled = true;
        public bool IsAutomaticPunctuationEnabled
        {
            get { return _isAutomaticPunctuationEnabled; }
            set { SetProperty(ref _isAutomaticPunctuationEnabled, value); }
        }


        #endregion

        #region Source file properties

        #region Properties

        #region Binded Properties

        private string _sourceFile;
        public string SourceFile
        {
            get { return _sourceFile; }
            set
            {
                SetProperty(ref _sourceFile, value);
                TranscribeCommand.RaiseCanExecuteChanged();
            }
        }

        private Uri _sourceFileURI;
        public Uri SourceFileURI
        {
            get { return _sourceFileURI; }
            set
            {
                SetProperty(ref _sourceFileURI, value);
                TranscribeCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isTranscriptionJobRunning = false;
        public bool IsTranscriptionJobRunning
        {
            get { return _isTranscriptionJobRunning; }
            set { SetProperty(ref _isTranscriptionJobRunning, value); }
        }

        #region Speech to text button

        private bool _isTranscribeButtonEnabled = false;
        public bool IsTranscribeButtonEnabled
        {
            get { return _isTranscribeButtonEnabled; }
            set
            {
                SetProperty(ref _isTranscribeButtonEnabled, value);
                CancelSpeechToTextCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

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

        #endregion

        #region File to choose from

        private bool _isLocalStorageEnabled = true;
        public bool IsLocalStorageEnabled
        {
            get { return _isLocalStorageEnabled; }
            set
            {
                SetProperty(ref _isLocalStorageEnabled, value);
                IsCloudStorageEnabled = !value;

                TranscribeCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isCloudStorageEnabled;
        public bool IsCloudStorageEnabled
        {
            get { return _isCloudStorageEnabled; }
            set { SetProperty(ref _isCloudStorageEnabled, value); }
        }


        #endregion



        #endregion

        #region Commands


        /// <summary>
        /// The command to control speech to text
        /// </summary>
        public DelegateCommand TranscribeCommand { get; set; }

        /// <summary>
        /// The command to stop (cancel) speech to text
        /// </summary>
        public DelegateCommand CancelSpeechToTextCommand { get; set; }

        public DelegateCommand ChooseFileCommand { get; set; }

        #endregion

        #endregion

        #endregion

        #region Supported Languages List

        private List<LanguageModel> _supportedLangauges = LanguageModel.GetSupportedLangaugesList();
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

        private readonly IEventAggregator _ea;

        #endregion

        #region Private Properties

        private readonly ICloudStorage _cloudStorage;
        private readonly ITranscriber _googleTranscriber;
        private CancellationTokenSource _cts;

        #endregion



        #endregion

        public SpeechToTextViewModel(IEventAggregator ea, ICloudStorage cloudStorage, ITranscriber googleTranscriber)
        {
            _ea = ea;
            _ea.GetEvent<StatusEvent>().Subscribe(NotifyStatus, ThreadOption.PublisherThread, false);

            _cloudStorage = cloudStorage;
            _googleTranscriber = googleTranscriber;

            _cts = new CancellationTokenSource();

            IsCloudStorageEnabled = !IsLocalStorageEnabled;

            SelectedLangauge = SupportedLangauges.FirstOrDefault(lang => lang.LanguageCode.Equals("en"));
            SelectedTranscriptionModel = SupportedTranscriptionModels.FirstOrDefault(model => model.ModelName.Equals("Default", StringComparison.OrdinalIgnoreCase));
            SelectedSpeakerDiarization = SpeakerDiarizationList.FirstOrDefault(x => x.Value == SpeakerDiarizationEnum.Off);
            SelectedSpeakerCount = SpeakersList.FirstOrDefault(x => x.Count == 2);


            ChooseFileCommand = new DelegateCommand(ChooseSourceFile);
            TranscribeCommand = new DelegateCommand(PerformSpeechToText, CanPerformSpeechToText);
            CancelSpeechToTextCommand = new DelegateCommand(CancelSpeechToText, CanCancelSpeechToText);


        }

        #region Methods

        #region Source File Methods

        private void ChooseSourceFile()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = $"Custom Files (*.opus,*.flac,*.webm,*.weba,*.wav,*.ogg,*.m4a,*.oga,*.mid,*.mp3,*.aiff,*.wma,*.au,*.ogm,*.wmv,*.mpg,*.webm,*.ogv,*.mov,*.asx,*.mpeg,*.mp4,*.m4v,*.avi)" +
                    $"|*.opus;*.flac;*.webm;*.weba;*.wav;*.ogg;*.m4a;*.oga;*.mid;*.mp3;*.aiff;*.wma;*.au;*.ogm;*.wmv;*.mpg;*.webm;*.ogv;*.mov;*.asx;*.mpeg;*.mp4;*.m4v;*.avi" +
                    $"|All Files (*.*)|*.*";
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SourceFile = dialog.FileName;
                }
            }

        }

        #region STT Command Methods

        private async void PerformSpeechToText()
        {
            IsTranscriptionJobRunning = true;
            TranscribeCommand.RaiseCanExecuteChanged();
            CancelSpeechToTextCommand.RaiseCanExecuteChanged();

            try
            {
                await Update("Performing Speech to Text...");

                ISpeechConfig speechConfig = new GoogeSpeechConfig()
                {
                    LanguageCode = SelectedLangauge.LanguageCode,
                    TranscriptionModel = SelectedTranscriptionModel.ModelValue,
                    AudioChannelCount = 1,
                    EnableAutomaticPunctuation = IsAutomaticPunctuationEnabled,
                    EnableSpeakerDiarization = SelectedSpeakerDiarization.Value == SpeakerDiarizationEnum.DownmixAndRecognizeMultipleSpeakers
                    || SelectedSpeakerDiarization.Value == SpeakerDiarizationEnum.RecognizeMultipleSpeakersInSingleChannel,
                    EnableSeparateRecognitionPerChannel = SelectedSpeakerDiarization.Value == SpeakerDiarizationEnum.Recognize1SpeakerPerChannel,
                    DiarizationSpeakerCount = SelectedSpeakerCount.Count,
                };

                string uri = string.Empty;

                if (IsLocalStorageEnabled)
                {
                    string audioFile = string.Empty;
                    // We don't need to convert flac/wav file
                    var fileExtension = Path.GetExtension(SourceFile);
                    if (!fileExtension.Equals(".flac") && !fileExtension.Equals(".wav"))
                    {
                        audioFile = await GerConvertedFilePath(SourceFile, _cts.Token);
                    }
                    else
                    {
                        audioFile = SourceFile;
                    }

                    if (!string.IsNullOrWhiteSpace(audioFile))
                    {
                        uri = await UploadAudioFileToCloudAsync(audioFile, _cts.Token);
                    }

                }
                else
                {
                    uri = SourceFileURI.ToString();
                }

                if (!string.IsNullOrWhiteSpace(uri))
                {
                    await Update("Transcribing");
                    await _googleTranscriber.TranscribeLongAudioFile(uri, speechConfig, _cts.Token);
                    await Update("Transcription completed");
                }
                await Update("", 2);
            }
            catch (TaskCanceledException) { }
            catch (Exception e)
            {
                await AppHelper.ShowMessage("Error", ExceptionHelper.ExtractExceptionMessage(e));
            }
            finally
            {
                IsTranscriptionJobRunning = false;
                TranscribeCommand.RaiseCanExecuteChanged();
                CancelSpeechToTextCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanPerformSpeechToText()
        {
            if (IsLocalStorageEnabled)
            {
                IsTranscribeButtonEnabled = !string.IsNullOrWhiteSpace(SourceFile) && !IsTranscriptionJobRunning;
            }
            else
            {
                IsTranscribeButtonEnabled = !string.IsNullOrWhiteSpace(SourceFileURI.ToString()) && !IsTranscriptionJobRunning;
            }
            return IsTranscribeButtonEnabled;
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
                    await Update("Canceling...");
                    _cts.Cancel(true);
                    await Update("Done", 1);
                    await Update(string.Empty);
                }
            }
            catch (Exception) { }
            finally
            {
                _cts?.Dispose(); // Clean up old token source....
                _cts = new CancellationTokenSource(); // "Reset" the cancellation token source...

                IsTranscriptionJobRunning = false;
            }
        }
        private bool CanCancelSpeechToText()
        {
            return IsCancelButtonEnabled = !IsTranscribeButtonEnabled && IsTranscriptionJobRunning;
        }

        #endregion

        #endregion

        private async Task<string> GerConvertedFilePath(string sourceFile, CancellationToken cancellationToken)
        {
            AudioConverter audioHelper = new AudioConverter(_ea);
            await Update($"Converting {Path.GetFileName(sourceFile)} to flac format");
            var isConverted = await audioHelper.ConvertToAudioAsync(SourceFile, cancellationToken);

            return isConverted ? audioHelper.TempOutputFile : string.Empty;
        }

        /// <summary>
        /// Returns the uploaded file cloud storage URI if the file uploaded successfully, else empty string.
        /// </summary>
        private async Task<string> UploadAudioFileToCloudAsync(string filePath, CancellationToken cancellationToken)
        {
            bool isUploadedSuccessfully = await _cloudStorage.UploadAsync(filePath,
                mimeType: "audio/flac", cancellationToken: cancellationToken);

            if (isUploadedSuccessfully)
            {
                return _cloudStorage.GetFileUri(Path.GetFileNameWithoutExtension(filePath));
            }
            return string.Empty;
        }

        private async void NotifyStatus(StatusEventParameters status)
        {
            await Update(status.Message);
        }

        private async Task Update(string message, int delayInSeconds = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
            StatusText = message;
        }

        #endregion

    }
}
