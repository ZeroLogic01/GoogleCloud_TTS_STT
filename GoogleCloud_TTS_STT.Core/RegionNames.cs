using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Core
{
    public static class RegionNames
    {
        public static string TextToSpeechRegion { get; private set; } = "GoogleTextToSpeechRegion";
        public static string SpeechToTextRegion { get; private set; } = "GoogleSpeechToTextRegion";
    }
}
