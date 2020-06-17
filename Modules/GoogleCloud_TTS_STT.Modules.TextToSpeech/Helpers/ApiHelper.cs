using ControlzEx.Standard;
using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Gets the output audio file extension based on <see cref="AudioEncoding"/>.
        /// </summary>
        internal static string GetAudioFileExtension(AudioEncoding audioEncoding)
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

        // [START tts_synthesize_text]
        /// <summary>
        /// Creates audio from the text input.
        /// </summary>
        /// <param name="text">Text to synthesize into audio</param>
        /// <remarks>
        /// Generates a file named 'output.mp3' in project folder.
        /// </remarks>
        internal static async Task SynthesizeText(string text, string languageCode, SsmlVoiceGender gender,
            string voiceName, AudioEncoding audioEncoding, float speakingRate, float pitch, int naturalSampleRateHertz)
        {
            TextToSpeechClient client = await TextToSpeechClient.CreateAsync();

            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
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
                AudioConfig = new AudioConfig()
                {
                    AudioEncoding = audioEncoding,
                    SpeakingRate = speakingRate,
                    Pitch = pitch
                }
            });

            //using (MemoryStream output = new MemoryStream())
            //{
            //    response.AudioContent.WriteTo(output);
            //}
            using (Stream output = File.Create($"output.{GetAudioFileExtension(audioEncoding)}"))
            {
                response.AudioContent.WriteTo(output);
            }


        }
        // [END tts_synthesize_text]
    }
}
