using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators
{
    public class ProgressBarEventParameters
    {
        public ProgressType Type { get; set; }
        public double Value { get; set; }
    }
}
