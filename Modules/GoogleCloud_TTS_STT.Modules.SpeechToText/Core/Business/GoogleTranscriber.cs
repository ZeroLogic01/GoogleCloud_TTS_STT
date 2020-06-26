using Google.Cloud.Speech.V1;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Interfaces;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Models;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business
{
    public class GoogleTranscriber : ITranscriber
    {
        private readonly IEventAggregator _eventAggregator;

        public GoogleTranscriber(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
        }

        public async Task TranscribeShortAudioFile(string localPath, ISpeechConfig speechConfig, CancellationToken cancellationToken)
        {
            if (speechConfig is GoogeSpeechConfig googleSpeechConfig)
            {
                var speech = await SpeechClient.CreateAsync(cancellationToken);
                var response = await speech.RecognizeAsync(GetRecognitionConfig(googleSpeechConfig),
                    RecognitionAudio.FromFile(localPath), cancellationToken);

                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        SaveTranscription(alternative.Transcript);
                    }
                }
            }
            else
            {
                throw new InvalidCastException($"{typeof(GoogeSpeechConfig)} is expected");
            }
        }


        public async Task TranscribeLongAudioFile(string storageUri, ISpeechConfig speechConfig, CancellationToken cancellationToken)
        {
            if (speechConfig is GoogeSpeechConfig googleSpeechConfig)
            {
                var speech = await SpeechClient.CreateAsync(cancellationToken);

                var longOperation = await speech.LongRunningRecognizeAsync(GetRecognitionConfig(googleSpeechConfig),
                    RecognitionAudio.FromStorageUri(storageUri), cancellationToken);

                longOperation = await longOperation.PollUntilCompletedAsync();

                var response = longOperation.Result;
                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        SaveTranscription(alternative.Transcript);
                    }
                }
            }
            else
            {
                throw new InvalidCastException($"{typeof(GoogeSpeechConfig)} is expected");
            }

        }

        private RecognitionConfig GetRecognitionConfig(GoogeSpeechConfig googleSpeechConfig)
        {
            var config = new RecognitionConfig();
            config.LanguageCode = googleSpeechConfig.LanguageCode;
            config.Model = googleSpeechConfig.TranscriptionModel;
            config.EnableAutomaticPunctuation = googleSpeechConfig.EnableAutomaticPunctuation;
            config.EnableSeparateRecognitionPerChannel = googleSpeechConfig.EnableSeparateRecognitionPerChannel;
            config.AudioChannelCount = googleSpeechConfig.AudioChannelCount;

            if (googleSpeechConfig.EnableSeparateRecognitionPerChannel)
            {
                config.DiarizationConfig = new SpeakerDiarizationConfig()
                {
                    EnableSpeakerDiarization = true,
                    MinSpeakerCount = googleSpeechConfig.DiarizationSpeakerCount,
                    MaxSpeakerCount = googleSpeechConfig.DiarizationSpeakerCount
                };
            }

            return config;
        }

        public void SaveTranscription(string transcript)
        {
            _eventAggregator.GetEvent<TranscriptEvent>().Publish(transcript);
        }
    }
}
