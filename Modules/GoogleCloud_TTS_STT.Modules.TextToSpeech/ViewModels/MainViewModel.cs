using Google.Cloud.TextToSpeech.V1;
using Google.Protobuf.WellKnownTypes;
using Google.Type;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Enums;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Helpers;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Models;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Static;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.ViewModels
{
    internal class MainViewModel : BindableBase
    {
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

        #endregion

        #region Commands

        /// <summary>
        /// The command to control text to speech
        /// </summary>
        public DelegateCommand TtsCommand { get; set; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            TtsCommand = new DelegateCommand(PerformTextToSpeech);

            _selectedAudioProfile = AudioProfiles.FirstOrDefault(x => x.Equals("Default", StringComparison.OrdinalIgnoreCase));
            _selectedAudioFormat = OutputAudioFormats.FirstOrDefault(x => x.AudioEncoding == AudioEncoding.Mp3);

            LoadVoiceData().ConfigureAwait(false);
        }

        #endregion

        #region Methods

        #region DataPopulation

        private async Task LoadVoiceData()
        {
            SupportedLangauges = new List<LanguageModel>();
            Genders = new List<string>();
            AvailableVoices = await LanguageHelper.GetAvailableVoices();

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

        }

        private void LoadVoiceTypes()
        {
            Genders = new List<string>();

            var collection = from m in AvailableVoices
                             select m;

            if (!string.IsNullOrWhiteSpace(SelectedLangauge.LanguageCode))
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

            if (!string.IsNullOrWhiteSpace(SelectedLangauge.LanguageCode))
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


        private async void PerformTextToSpeech()
        {
            if (string.IsNullOrWhiteSpace(TextToSpeech))
            {
                return;
            }

            SsmlVoiceGender gender = (SsmlVoiceGender)System.Enum.Parse(typeof(SsmlVoiceGender), SelectedGender);

            var naturalSampleRateHertz = AvailableVoices.Where(x => x.Name.Equals(SelectedVoice))
                .Select(x => x.NaturalSampleRateHertz).FirstOrDefault();

            await ApiHelper.SynthesizeText(text: TextToSpeech, languageCode: SelectedLangauge.LanguageCode,
                gender: gender, voiceName: SelectedVoice, audioEncoding: SelectedAudioFormat.AudioEncoding,
                speakingRate: Speed, pitch: Pitch, naturalSampleRateHertz: naturalSampleRateHertz);

            //  throw new NotImplementedException();
        }

        #endregion
    }
}
