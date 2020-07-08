using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Core
{
    public static class AppConstants
    {
        public static string GOOGLE_APPLICATION_CREDENTIALS { get; private set; } = "GOOGLE_APPLICATION_CREDENTIALS";
        public static int MaximumNumberOfSsmlCharactersAllowed_TTS { get; private set; } = 4985;
        public static int MaximumNumberOfTextCharactersAllowed_TTS { get; private set; } = 5000;
    }
}
