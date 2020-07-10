using EnumsNET;
using Google.Cloud.TextToSpeech.V1;
using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Enums;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Helpers;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Models;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.SSML;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Static;
using MahApps.Metro.SimpleChildWindow;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.ViewModels
{
    internal class TextToSpeechViewModel : BindableBase
    {
        #region Properties

        private bool _isTextOptionSelected = true;
        public bool IsTextOptionSelected
        {
            get { return _isTextOptionSelected; }
            set
            {
                SetProperty(ref _isTextOptionSelected, value);
            }
        }



        private string _text;

        /// <summary>
        /// The text to be converted to speech
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                SetProperty(ref _text, value);
                IsTextToSpeechButtonEnabled = !string.IsNullOrWhiteSpace(value);
                TtsCommand.RaiseCanExecuteChanged();
            }
        }

        private string _ssml;

        /// <summary>
        /// The SSML to be converted to speech
        /// </summary>
        public string SSML
        {
            get { return _ssml; }
            set
            {
                SetProperty(ref _ssml, value);
                IsTextToSpeechButtonEnabled = !string.IsNullOrWhiteSpace(value);
                TtsCommand.RaiseCanExecuteChanged();
            }
        }

        private string _selectedText = string.Empty;
        public string SelectedText
        {
            get { return _selectedText; }
            set
            {
                SetProperty(ref _selectedText, value);
                SsmlEmphasizeCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isTextToSpeechButtonEnabled = false;
        public bool IsTextToSpeechButtonEnabled
        {
            get { return _isTextToSpeechButtonEnabled; }
            set { SetProperty(ref _isTextToSpeechButtonEnabled, value); }
        }

        /// <summary>
        /// Text to speech button content
        /// </summary>
        private string _buttonContent = TtsButtonContent.SpeakIt;
        public string ButtonContent
        {
            get { return _buttonContent; }
            set { SetProperty(ref _buttonContent, value); }
        }

        private float _speed = 1.00f;
        public float Speed
        {
            get { return _speed; }
            set { SetProperty(ref _speed, value); }
        }

        private float _pitch = 0.00f;
        public float Pitch
        {
            get { return _pitch; }
            set { SetProperty(ref _pitch, value); }
        }


        private List<OutputAudio> _outputAudioFormats = OutputAudio.GetSupportedAudioFormats();
        public List<OutputAudio> OutputAudioFormats
        {
            get { return _outputAudioFormats; }
            set { SetProperty(ref _outputAudioFormats, value); }
        }

        private OutputAudio _selectedAudioFormat;
        public OutputAudio SelectedAudioFormat
        {
            get { return _selectedAudioFormat; }
            set { SetProperty(ref _selectedAudioFormat, value); }
        }

        private List<string> _audioProfiles = ApiHelper.GetAvailableAudioProfiles();
        public List<string> AudioProfiles
        {
            get { return _audioProfiles; }
            set { SetProperty(ref _audioProfiles, value); }
        }

        private string _selectedAudioProfile;
        public string SelectedAudioProfile
        {
            get { return _selectedAudioProfile; }
            set { SetProperty(ref _selectedAudioProfile, value); }
        }

        private List<LanguageModel> _supportedLangauges;
        public List<LanguageModel> SupportedLangauges
        {
            get { return _supportedLangauges; }
            set { SetProperty(ref _supportedLangauges, value); }
        }

        private LanguageModel _selectedLangauge;
        public LanguageModel SelectedLangauge
        {
            get { return _selectedLangauge; }
            set
            {
                SetProperty(ref _selectedLangauge, value);
                LoadVoiceTypes();
                LoadVoiceNames();
            }
        }

        private List<Voice> _availableVoices;
        public List<Voice> AvailableVoices
        {
            get { return _availableVoices; }
            set { SetProperty(ref _availableVoices, value); }
        }

        private List<string> _genders;
        public List<string> Genders
        {
            get { return _genders; }
            set { SetProperty(ref _genders, value); }
        }

        private string _selectedGender;
        public string SelectedGender
        {
            get { return _selectedGender; }
            set
            {
                SetProperty(ref _selectedGender, value);
                LoadVoiceNames();
            }
        }

        private List<string> _voiceNames;
        public List<string> VoiceNames
        {
            get { return _voiceNames; }
            set { SetProperty(ref _voiceNames, value); }
        }

        private string _selectedVoice;
        public string SelectedVoice
        {
            get { return _selectedVoice; }
            set { SetProperty(ref _selectedVoice, value); }
        }

        private Visibility _noInternetConnectionPanelVisibility = Visibility.Collapsed;
        public Visibility NoInternetConnectionPanelVisibility
        {
            get { return _noInternetConnectionPanelVisibility; }
            set { SetProperty(ref _noInternetConnectionPanelVisibility, value); }
        }

        #region Status Text



        private string _statusText = string.Empty;
        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }


        #endregion


        #endregion

        #region Commands

        /// <summary>
        /// The command to control text to speech
        /// </summary>
        public DelegateCommand TtsCommand { get; set; }
        public DelegateCommand ReloadComboboxesCommand { get; set; }

        #region SSML Commands

        public DelegateCommand SsmlBreakCommand { get; set; }
        public DelegateCommand<string> SsmlEmphasizeCommand { get; set; }

        #endregion

        #endregion

        #region Constructor

        public TextToSpeechViewModel()
        {
            //  ReloadComboboxesCommand = new DelegateCommand(ReloadComboboxes);

            TtsCommand = new DelegateCommand(ExecuteTextToSpeech, CanExecuteTextToSpeech);
            SsmlBreakCommand = new DelegateCommand(ExecuteSsmlBreak);
            SsmlEmphasizeCommand = new DelegateCommand<string>(ExecuteSsmlExphasize, CanExecuteSsmlExphasize);

            _selectedAudioProfile = AudioProfiles.FirstOrDefault(x => x.Equals("Default", StringComparison.OrdinalIgnoreCase));
            _selectedAudioFormat = OutputAudioFormats.FirstOrDefault(x => x.AudioEncoding == AudioEncoding.Mp3);

            if (!AppHelper.CheckForInternetConnection())
            {
                NoInternetConnectionPanelVisibility = Visibility.Visible;
            }
            LoadVoiceData().ConfigureAwait(false);
        }


        #endregion

        #region Methods

        #region DataPopulation

        private async Task LoadVoiceData()
        {
            try
            {
                await UpdateApplicationStatus("Fetching supported Languages/Locales list from the api...");
                AvailableVoices = await LanguageHelper.GetAvailableVoices();
                SupportedLangauges = new List<LanguageModel>();

                if (NoInternetConnectionPanelVisibility == Visibility.Visible)
                {
                    NoInternetConnectionPanelVisibility = Visibility.Collapsed;
                }

                foreach (var voice in AvailableVoices)
                {
                    foreach (var languageCode in voice.LanguageCodes)
                    {
                        string displayName = ApiHelper.GetLanguageDisplayName(languageCode);
                        if (!SupportedLangauges.Any(x => x.LanguageCode.Equals(languageCode)))
                        {
                            SupportedLangauges.Add(new LanguageModel { LanguageName = displayName, LanguageCode = languageCode });
                        }
                    }
                }
                SupportedLangauges = SupportedLangauges.OrderBy(order => order.LanguageName).Distinct().ToList();
                SelectedLangauge = SupportedLangauges.FirstOrDefault(x => x.LanguageCode.Equals("en-GB"));
                await UpdateApplicationStatus("Ready", .5);
            }
            catch (Exception ex)
            {
                string errorMessage = $"{ExceptionHelper.ExtractExceptionMessage(ex)}";
                await AppHelper.ShowMessage("Error", errorMessage);
                await UpdateApplicationStatus("Failed to fetch supported languages.", .5);
            }
        }

        private void LoadVoiceTypes()
        {
            Genders = new List<string>();

            var collection = from m in AvailableVoices
                             select m;

            if (!string.IsNullOrWhiteSpace(SelectedLangauge?.LanguageCode))
            {
                collection = collection.Where(x => x.LanguageCodes.Contains(SelectedLangauge.LanguageCode));
            }

            Genders = collection.Select(x => x.SsmlGender.ToString()).Distinct().ToList();
            SelectedGender = Genders?.FirstOrDefault();
        }

        private void LoadVoiceNames()
        {
            VoiceNames = new List<string>();

            var collection = from m in AvailableVoices
                             select m;

            if (!string.IsNullOrWhiteSpace(SelectedLangauge?.LanguageCode))
            {
                collection = collection.Where(x => x.LanguageCodes.Contains(SelectedLangauge.LanguageCode));
            }

            if (!string.IsNullOrWhiteSpace(SelectedGender))
            {
                collection = collection.Where(x => x.SsmlGender.ToString().Equals(SelectedGender));
            }

            VoiceNames = collection.Select(x => x.Name).ToList();
            SelectedVoice = VoiceNames?.FirstOrDefault();
        }

        #endregion

        #region Commands implementation methods

        private void ReloadComboboxes()
        {
            UpdateApplicationStatus("Reloading Language/Locale list...");
            LoadVoiceData().ConfigureAwait(false);
        }

        private async void ExecuteTextToSpeech()
        {
            IsTextToSpeechButtonEnabled = false;
            TtsCommand.RaiseCanExecuteChanged();

            try
            {
                SynthesisType synthesisType;
                string inputToBeTranscribed = string.Empty;
                if (IsTextOptionSelected)
                {
                    synthesisType = SynthesisType.TextToSpeech;
                    inputToBeTranscribed = Text;
                }
                else
                {
                    synthesisType = SynthesisType.SSMLToSpeech;
                    inputToBeTranscribed = $"<speak>{SSML}<speak>";
                }

                await UpdateApplicationStatus($"Performing {synthesisType.AsString(EnumFormat.Description)}", .5);
                SsmlVoiceGender gender = (SsmlVoiceGender)Enum.Parse(typeof(SsmlVoiceGender), SelectedGender);

                await ApiHelper.SynthesizeTextAndSaveToFile(text: inputToBeTranscribed, languageCode: SelectedLangauge.LanguageCode,
                       gender: gender, voiceName: SelectedVoice, audioEncoding: SelectedAudioFormat.AudioEncoding,
                       speakingRate: Speed, pitch: Pitch, effectProfileId: SelectedAudioProfile, synthesisType);

                await UpdateApplicationStatus("Done", .5);
            }
            catch (Exception e)
            {
                await AppHelper.ShowMessage("Error", ExceptionHelper.ExtractExceptionMessage(e));
            }

            IsTextToSpeechButtonEnabled = !string.IsNullOrWhiteSpace(Text) || !string.IsNullOrWhiteSpace(SSML);
            TtsCommand.RaiseCanExecuteChanged();
        }


        private bool CanExecuteTextToSpeech()
        {
            bool isInputEmpty;
            if (IsTextOptionSelected)
            {
                isInputEmpty = string.IsNullOrWhiteSpace(Text);
            }
            else
            {
                isInputEmpty = string.IsNullOrWhiteSpace(SSML);
            }

            return IsTextToSpeechButtonEnabled && !isInputEmpty && SelectedLangauge != null
                   && !string.IsNullOrWhiteSpace(SelectedGender) && !string.IsNullOrWhiteSpace(SelectedVoice);
        }


        private async void ExecuteSsmlBreak()
        {
            await Application.Current.MainWindow.ShowChildWindowAsync<bool>(new BreakTag() { IsModal = true, AllowMove = true }, ChildWindowManager.OverlayFillBehavior.WindowContent);
        }


        private async void ExecuteSsmlExphasize(string level)
        {
            string emphasizedText = $"<emphasis level=\"{level}\">{SelectedText}</emphasis>";

            int totalLenghtWithoutSelectedText = SSML.Length - SelectedText.Length;

            // insert only if the sum of totalLenghtWithoutSelectedText & emphasizedText characters is less than equals to 
            // MaximumNumberOfSsmlCharactersAllowed_TTS
            if (totalLenghtWithoutSelectedText + emphasizedText.Length <= AppConstants.MaximumNumberOfSsmlCharactersAllowed_TTS)
            {
                SelectedText = emphasizedText;
            }
            else
            {
                await AppHelper.ShowMessage("Maximum length will be exceeded!", "Cannot insert a new emphasis tag because the length of remaining" +
                    " characters allowed is less than the number of characters required for emphasis tag.");
            }
        }
        private bool CanExecuteSsmlExphasize(string arg)
        {
            return !string.IsNullOrWhiteSpace(_selectedText);
        }




        #endregion

        public async Task UpdateApplicationStatus(string message, double delayInSeconds = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
            StatusText = message;
        }

        #endregion
    }

}
