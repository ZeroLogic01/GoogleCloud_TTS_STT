using GoogleCloud_TTS_STT.Modules.TextToSpeech.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.Enums
{
    //[TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum BreakTagAttribute
    {
        [Description(@"Time")]
        Time = 0,
        [Description(@"Strength")]
        Strength = 1
    }
}
