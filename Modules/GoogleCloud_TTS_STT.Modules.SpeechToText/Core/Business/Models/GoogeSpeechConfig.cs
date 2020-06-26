using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Interfaces;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Models;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Models
{
    public class GoogeSpeechConfig : ISpeechConfig
    {
        public string LanguageCode { get; set; }
        public string TranscriptionModel { get; set; }
        public bool EnableAutomaticPunctuation { get; set; }
        public bool EnableSeparateRecognitionPerChannel { get; set; }
        public int AudioChannelCount { get; set; } = 1;
        public bool EnableSpeakerDiarization { get; set; }
        public int DiarizationSpeakerCount { get; set; }

    }
}
