using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Enums;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators;
using NReco.VideoConverter;
using Prism.Events;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Helpers
{
    class AudioConverter
    {
        #region Private Fields

        private static readonly string _appDataRootDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}" +
              @"\Balconette Speech Services\STT\Data\";
        private static readonly string _uniqueFileName = GetUniqueFileName();
        private readonly IEventAggregator _eventAggregator;

        #endregion


        #region Public Properties

        public string DefaultFormat { get; } = Format.flac;

        public string TempOutputFile { get; private set; }


        #endregion

        public AudioConverter(IEventAggregator eventAggregator)
        {
            TempOutputFile = $"{_appDataRootDir}{_uniqueFileName}.{DefaultFormat}";
            _eventAggregator = eventAggregator;
        }

        #region Methods

        #region Static Methods

        public static void CleanTempDataOnStartUp()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(_appDataRootDir);

                if (!di.Exists)
                {
                    return;
                }

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch { }
        }


        /// <summary>
        /// Gets a globally unique file name.
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueFileName()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion


        public async Task<bool> ConvertToAudioAsync(string sourceFile,
            CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(_appDataRootDir);

            var ffMpeg = new FFMpegConverter();
            ffMpeg.ConvertProgress += FfMpeg_ConvertProgress;

            UpdateProgressBar(ProgressType.ProgressBarMinimumValue, 0);
            UpdateProgressBar(ProgressType.ProgressBarMaximumValue, 100);


         //   Console.WriteLine(TempOutputFile);

            var cancellationTokenRegisteration = cancellationToken.Register(() =>
            {
                if (!ffMpeg.Stop()) { ffMpeg.Abort(); }
            });

            await Task.Run(() =>
              {
                  try
                  {
                      ffMpeg.ConvertMedia(sourceFile,
                         TempOutputFile, DefaultFormat);
                  }
#pragma warning disable CA1031 // Do not catch general exception types
                  catch (NullReferenceException) { }
#pragma warning restore CA1031 // Do not catch general exception types
                  catch (Exception ex)
                  {
                      throw ex;
                  }
                  finally
                  {
                      ffMpeg.ConvertProgress -= FfMpeg_ConvertProgress;
                  }

              }, cancellationToken);

            return !cancellationToken.IsCancellationRequested;
        }

        private void FfMpeg_ConvertProgress(object sender, ConvertProgressEventArgs e)
        {
            var currentValue = e.Processed.TotalSeconds * 100 / e.TotalDuration.TotalSeconds;
            UpdateProgressBar(ProgressType.ProgressBarCurrentValue, currentValue);
        }

        private void UpdateProgressBar(ProgressType type, double value = default)
        {
            _eventAggregator.GetEvent<ProgressBarEvent>().Publish(new ProgressBarEventParameters()
            {
                Type = type,
                Value = value
            });
        }


        /// <summary>
        /// Deletes the temporary created WAV files. Ensure that to call it when <see cref="TempOutputFileTwoPath"/>
        /// is not being used by the application (i.e. uploading to the Google cloud storage).
        /// </summary>
        public void DeleteTemporaryFiles()
        {
            File.Delete(TempOutputFile);
        }

        #endregion
    }
}
