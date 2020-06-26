using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators
{
    public class TranscriptEvent : PubSubEvent<string>
    {
    }
}
