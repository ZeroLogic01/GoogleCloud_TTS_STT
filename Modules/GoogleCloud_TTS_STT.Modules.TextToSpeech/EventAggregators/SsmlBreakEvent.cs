using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.EventAggregators
{
    public class SsmlBreakEvent : PubSubEvent<string>
    {
    }
}
