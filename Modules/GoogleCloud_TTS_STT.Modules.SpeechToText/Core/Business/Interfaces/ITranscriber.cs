using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Interfaces
{
    public interface ITranscriber
    {
        Task TranscribeShortAudioFile(string localPath, ISpeechConfig speechConfig, CancellationToken cancellationToken);


        Task TranscribeLongAudioFile(string storageUri, ISpeechConfig speechConfig, CancellationToken cancellationToken);


        void SaveTranscription(string transcript);
    }


}
