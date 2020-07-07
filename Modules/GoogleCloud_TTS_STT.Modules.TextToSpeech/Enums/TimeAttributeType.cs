using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.Enums
{
    public enum TimeAttributeType
    {
        [Description(@"Seconds")]
        s = 0,

        [Description(@"Milliseconds")]
        ms = 1
    }
}
