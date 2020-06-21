using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Models
{
    public class SpeakerDiarization
    {
        public string DisplayName { get; set; }
        public SpeakerDiarizationEnum Value { get; set; }
    }

    public enum SpeakerDiarizationEnum
    {
        Off = 0,
        DownmixAndRecognizeMultipleSpeakers = 1,
        RecognizeMultipleSpeakersInSingleChannel = 2,
        Recognize1SpeakerPerChannel = 3
    }
}
