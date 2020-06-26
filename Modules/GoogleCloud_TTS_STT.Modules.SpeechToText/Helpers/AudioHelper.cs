using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Helpers
{
    class AudioHelper
    {
        #region Private Fields

        private static readonly string _appDataRootDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}" +
              @"\Audio to Text Transcriber\Data\";
        private static readonly string _uniqueFileName = GetUniqueFileName();

        #endregion


        #region Public Properties

        /// <summary>
        /// Temporary WAV output file (converted from source file to the WAV file) one path (large sized).
        /// </summary>
        public string TempOutputFileOnePath { get; private set; } = $"{_appDataRootDir}{_uniqueFileName}_1.wav";

        /// <summary>
        /// Temporary WAV output file (converted from "TempOutputFileOne" to an another WAV file) two path (small sized).
        /// </summary>
        public string TempOutputFileTwoPath { get; private set; } = $"{_appDataRootDir}{_uniqueFileName}_2.wav";

        #endregion

        #region Methods

        public static void CleanTempDataOnStartUp()
        {

            DirectoryInfo di = new DirectoryInfo(_appDataRootDir);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }


        /// <summary>
        /// Gets a globally unique file name.
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueFileName()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Determines the total duration of the audio file 
        /// &amp; returns true if duration is less than 60 seconds.
        /// </summary>
        /// <param name="audioFilePath"></param>
        /// <returns></returns>
        public bool IsShortAudioFile(string audioFilePath)
        {
            // set the default value to false
            //bool isShortAudio = false;

            //using (var shell = ShellObject.FromParsingName(audioFilePath))
            //{
            //    IShellProperty prop = shell.Properties.System.Media.Duration;
            //    var t = (ulong)prop.ValueAsObject;

            //    var totalDurationInSeconds = TimeSpan.FromTicks((long)t).TotalSeconds;
            //    if (totalDurationInSeconds < 60)
            //    {
            //        isShortAudio = true;
            //    }
            //}

            return false;
        }


        /// <summary>
        /// Deletes the temporary created WAV files. Ensure that to call it when <see cref="TempOutputFileTwoPath"/>
        /// is not being used by the application (i.e. uploading to the Google cloud storage).
        /// </summary>
        public void DeleteTemporaryFiles()
        {
            File.Delete(TempOutputFileOnePath);
            File.Delete(TempOutputFileTwoPath);
        }

        #endregion
    }
}
