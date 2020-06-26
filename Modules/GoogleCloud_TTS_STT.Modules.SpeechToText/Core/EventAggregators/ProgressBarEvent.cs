using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators
{
    public class ProgressBarEvent : PubSubEvent<ProgressBarEventParameters>
    {
    }
}
