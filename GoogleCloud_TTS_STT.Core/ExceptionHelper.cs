using System;
using System.Text;

namespace GoogleCloud_TTS_STT.Core
{
    public static class ExceptionHelper
    {
        public static string ExtractExceptionMessage(Exception ex)
        {
            StringBuilder exceptionText = new StringBuilder();
            exceptionText.Append(ex.Message);
            while (ex.InnerException != null)
            {
                exceptionText.Append($" {ex.InnerException.Message}");
                ex = ex.InnerException;
            }
            return exceptionText.ToString();
        }
    }
}
