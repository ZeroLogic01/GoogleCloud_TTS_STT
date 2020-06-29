using Google.Cloud.TextToSpeech.V1;
using Google.Protobuf;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.Helpers
{
    internal static class ApiHelper
    {
        public static List<string> GetAvailableAudioProfiles()
        {
            return new List<string>()
            {
                "Default",
                "wearable-class-device",
                "handset-class-device",
                "headphone-class-device",
                "small-bluetooth-speaker-class-device",
                "medium-bluetooth-speaker-class-device",
                "large-home-entertainment-class-device",
                "large-automotive-class-device",
                "telephony-class-application"
            };
        }

        public static string GetLanguageDisplayName(string languageCode)
        {
            try
            {
                var culture = new CultureInfo(languageCode);
                string displayName = culture.DisplayName;
                if (displayName.Contains("Unknown Locale"))
                {
                    switch (languageCode)
                    {
                        case "cmn-CN":
                            displayName = "Chinese Mandarin (Simplified China)";
                            break;
                        case "cmn-TW":
                            displayName = "Chinese Mandarin (Traditional Taiwan)";
                            break;
                        default:
                            displayName = new CultureInfo(languageCode.Split('-')[0]).DisplayName;
                            break;
                    }
                }
                return displayName;
            }
            catch
            {
                return languageCode;
            }
        }

        /// <summary>
        /// Gets the output audio file extension based on <see cref="AudioEncoding"/>.
        /// </summary>
        internal static string GetFileExtension(AudioEncoding audioEncoding)
        {
            switch (audioEncoding)
            {
                case AudioEncoding.Linear16:
                    return "wav";
                case AudioEncoding.Mp3:
                    return "mp3";
                case AudioEncoding.OggOpus:
                    return "opus";
                default:
                    return audioEncoding.ToString();
            }
        }

        internal static string GetSaveFileDialogFilter(AudioEncoding audioEncoding)
        {
            switch (audioEncoding)
            {
                case AudioEncoding.Linear16:
                    return "Wave file (*.wav)|*.wav";
                case AudioEncoding.Mp3:
                    return "Mp3 file (*.mp3)|*.mp3";
                case AudioEncoding.OggOpus:
                    return "Ogg/Opus file (*.opus)|*.opus";
                default:
                    return audioEncoding.ToString();
            }
        }


        internal static async Task SynthesizeTextAndSaveToFile(string text, string languageCode, SsmlVoiceGender gender,
           string voiceName, AudioEncoding audioEncoding, float speakingRate, float pitch, string effectProfileId)
        {
            var audioBytes = await SynthesizeText(text, languageCode, gender, voiceName, audioEncoding, speakingRate, pitch, effectProfileId);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = GetSaveFileDialogFilter(audioEncoding)
            };

            saveFileDialog.FileOk += (object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                string fileExtension = GetFileExtension(audioEncoding);
                if (Path.GetExtension(saveFileDialog.FileName).ToLower() != $".{fileExtension}")
                {
                    e.Cancel = true;
                    MessageBox.Show($"Please omit the extension or use '{fileExtension}'", "Wrong extension", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            };

            if (saveFileDialog.ShowDialog() == false)
            {
                return;
            }

            using (Stream output = File.Create(saveFileDialog.FileName))
            {
                audioBytes.WriteTo(output);
            }

            try
            {
                Process.Start(saveFileDialog.FileName);
            }
            catch { }

        }


        /// <summary>
        /// Creates audio from the text input.
        /// </summary>
        internal static async Task<ByteString> SynthesizeText(string text, string languageCode, SsmlVoiceGender gender,
        string voiceName, AudioEncoding audioEncoding, float speakingRate, float pitch, string effectProfileId)
        {

            TextToSpeechClient client = await TextToSpeechClient.CreateAsync();

            var response = await client.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Text = text
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = languageCode,
                    SsmlGender = gender,
                    Name = voiceName
                },
                AudioConfig = effectProfileId.Equals("Default") ? new AudioConfig()
                {
                    AudioEncoding = audioEncoding,
                    SpeakingRate = speakingRate,
                    Pitch = pitch
                } :
                new AudioConfig()
                {
                    AudioEncoding = audioEncoding,
                    SpeakingRate = speakingRate,
                    Pitch = pitch,
                    EffectsProfileId = { effectProfileId }
                }
            });

            return response.AudioContent;


        }

    }
}
