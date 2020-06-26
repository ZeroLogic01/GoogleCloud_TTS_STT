using Google.Cloud.Speech.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Models
{
    public class LanguageModel
    {
        public string LanguageName { set; get; }
        public string LanguageCode { set; get; }


        /// <summary>
        /// Gets list the all supported languages by "Google speech to text API".
        /// </summary>
        /// <returns></returns>
        public static List<LanguageModel> GetSupportedLangaugesList()
        {
            List<LanguageModel> languages = new List<LanguageModel>();

            // extract list of supported languages from LanguageCodes' nested classes
            foreach (var type in typeof(LanguageCodes).GetNestedTypes(BindingFlags.Public))
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

            languages.Add(new LanguageModel()
            {
                LanguageName = "English",
                LanguageCode = "en"
            });
            return languages.OrderBy(item => item.LanguageName).ToList();
        }
    }
}
