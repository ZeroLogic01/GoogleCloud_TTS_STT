using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.Enums
{
    public enum SynthesisType
    {
        [Description("text to speech")]
        TextToSpeech = 0,
        [Description("ssml to speech")]
        SSMLToSpeech = 1
    }
}
