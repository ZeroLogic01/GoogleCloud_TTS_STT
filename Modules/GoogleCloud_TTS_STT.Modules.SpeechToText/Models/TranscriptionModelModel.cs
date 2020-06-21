using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Models
{
    /// <summary>
    /// The Transcription Models supported by the Text-to-Speech API.
    /// </summary>
    public class TranscriptionModelModel
    {
        public string ModelName { set; get; }
        public string ModelValue { set; get; }
    }
}
