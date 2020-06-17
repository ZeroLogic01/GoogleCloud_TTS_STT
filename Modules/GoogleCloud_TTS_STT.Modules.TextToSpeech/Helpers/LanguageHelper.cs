using Google.Cloud.TextToSpeech.V1;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.Helpers
{
    internal class LanguageHelper
    {
        /// <summary>
        /// Gets list the all supported languages by "Google speech to text API".
        /// </summary>
        /// <returns></returns>
        public static List<LanguageModel> GetSupportedLangaugesList()
        {
            List<LanguageModel> languages = new List<LanguageModel>();

            // extract list of supported languages from LanguageCodes' nested classes
            foreach (var type in typeof(AudioEncoding).GetNestedTypes(BindingFlags.Public))
            {
                if (!type.IsAbstract)
                {
                    continue;
                }
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {

                    languages.Add(new LanguageModel()
                    {
                        LanguageName = $"{type.Name} ({field.Name})",
                        LanguageCode = field.GetValue(null).ToString()
                    });
                }
            }

            return languages.OrderBy(item => item.LanguageName).ToList();
        }

        // [START tts_list_voices]
        /// <summary>
        /// Lists all the voices available for speech synthesis.
        /// </summary>
        /// <param name="desiredLanguageCode">Language code to filter on</param>
        public static async Task<List<Voice>> GetAvailableVoices(string desiredLanguageCode = "")
        {
            TextToSpeechClient client = TextToSpeechClient.Create();

            var response = await client.ListVoicesAsync(new ListVoicesRequest
            {
                LanguageCode = desiredLanguageCode
            });

            return response.Voices.ToList();
        }


    }
}
