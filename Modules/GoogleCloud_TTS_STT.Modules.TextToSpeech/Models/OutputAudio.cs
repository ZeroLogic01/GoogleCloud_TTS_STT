using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.Models
{
    internal class OutputAudio
    {
        public string Name { get; set; }
        public AudioEncoding AudioEncoding { get; set; }


        public static List<OutputAudio> GetSupportedAudioFormats()
        {
            List<OutputAudio> list = new List<OutputAudio>();

            foreach (var encoding in Enum.GetValues(typeof(AudioEncoding)))
            {
                if (!encoding.ToString().Equals("Unspecified", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(new OutputAudio { Name = encoding.ToString(), AudioEncoding = (AudioEncoding)encoding });
                }
            }

            return list;
        }
    }
}