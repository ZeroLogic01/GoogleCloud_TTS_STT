﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Regions
{
    public static class RegionNames
    {
        public static string TranscriptionRegion { get; private set; } = "TranscriptionRegion";
        public static string SpeechToTextAPIConfigRegion { get; private set; } = "SpeechToTextAPIConfigRegion";
        public static string ProgessBarRegion { get; private set; } = "ProgessBarRegion";
    }
}
