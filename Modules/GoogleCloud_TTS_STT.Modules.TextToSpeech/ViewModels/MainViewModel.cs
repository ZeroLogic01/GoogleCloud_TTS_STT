using Google.Cloud.TextToSpeech.V1;
using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Core.EventAggregators;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Helpers;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Models;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Static;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.ViewModels
{
    internal class MainViewModel : BindableBase
    {
        #region Private fields
        private readonly IEventAggregator _ea;
        #endregion

        #region Properties

        private string _textToSpeech;
        public string TextToSpeech
        {
            get { return _textToSpeech; }
            set { SetProperty(ref _textToSpeech, value); }
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



        #endregion

        #region Commands

        /// <summary>
        /// The command to control text to speech
        /// </summary>
        public DelegateCommand TtsCommand { get; set; }
        public DelegateCommand ReloadComboboxesCommand { get; set; }




        #endregion

        #region Constructor

        public MainViewModel()
        {

        }

        public MainViewModel(IEventAggregator ea)
        {
            _ea = ea;

            TtsCommand = new DelegateCommand(PerformTextToSpeech);
            ReloadComboboxesCommand = new DelegateCommand(ReloadComboboxes);

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
                UpdateApplicationStatus("Fetching supported Languages/Locales list from the api...");
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
                UpdateApplicationStatus("Ready");
            }
            catch (Exception ex)
            {
                string errorMessage = $"{ExceptionHelper.ExtractExceptionMessage(ex)}";
                await ApplicationHelper.ShowMessage("Error", errorMessage, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary);
                UpdateApplicationStatus("Failed to fetch supported languages.");
            }
        }

        private void LoadVoiceTypes()
        {
            UpdateApplicationStatus("Extracting voice types...");
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
            UpdateApplicationStatus("Extracting voice names...");
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

        private void ReloadComboboxes()
        {
            UpdateApplicationStatus("Reloading Language/Locale list...");
            LoadVoiceData().ConfigureAwait(false);
        }

        private async void PerformTextToSpeech()
        {
            if (string.IsNullOrWhiteSpace(TextToSpeech) || SelectedLangauge == null
                || string.IsNullOrWhiteSpace(SelectedGender) || string.IsNullOrWhiteSpace(SelectedVoice))
            {
                return;
            }

            try
            {
                SsmlVoiceGender gender = (SsmlVoiceGender)Enum.Parse(typeof(SsmlVoiceGender), SelectedGender);

                await ApiHelper.SynthesizeTextAndSaveToFile(text: TextToSpeech, languageCode: SelectedLangauge.LanguageCode,
                       gender: gender, voiceName: SelectedVoice, audioEncoding: SelectedAudioFormat.AudioEncoding,
                       speakingRate: Speed, pitch: Pitch, effectProfileId: SelectedAudioProfile);

                //var naturalSampleRateHertz = AvailableVoices.Where(x => x.Name.Equals(SelectedVoice))
                //   .Select(x => x.NaturalSampleRateHertz).FirstOrDefault();

            }
            catch (Exception e)
            {
                await ApplicationHelper.ShowMessage("Error", ExceptionHelper.ExtractExceptionMessage(e));
            }
        }



        internal void CopyToFile(MemoryStream input)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = ApiHelper.GetFileExtension(SelectedAudioFormat.AudioEncoding)
            };

            if (saveFileDialog.ShowDialog() == false)
            {
                return;
            }
            // It won't matter if we throw an exception during this method;
            // we don't *really* need to dispose of the MemoryStream, and the
            // caller should dispose of the input stream
            Stream ret = File.Create(saveFileDialog.FileName);

            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ret.Write(buffer, 0, bytesRead);
            }
            // Rewind ready for reading (typical scenario)
            ret.Position = 0;
        }

        public void UpdateApplicationStatus(string message)
        {
            _ea.GetEvent<StatusTextEvent>().Publish(
                new StatusTextEventParameters
                {
                    Message = message
                });
        }

        #endregion
    }

}
