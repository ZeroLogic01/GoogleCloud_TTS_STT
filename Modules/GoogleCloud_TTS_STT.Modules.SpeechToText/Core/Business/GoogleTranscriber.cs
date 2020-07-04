using Google.Cloud.Speech.V1;
using Google.Protobuf.Collections;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Interfaces;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Models;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Helpers;
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

                HandleSpeechRecognitionResults(response.Results, googleSpeechConfig.EnableSpeakerDiarization);
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

                HandleSpeechRecognitionResults(response.Results, googleSpeechConfig.EnableSpeakerDiarization);
            }
            else
            {
                throw new InvalidCastException($"{typeof(GoogeSpeechConfig)} is expected");
            }

        }


        public void HandleSpeechRecognitionResults(RepeatedField<SpeechRecognitionResult> results, bool isSpeakerDiarizationEnabled)
        {
            if (results.Count == 0)
            {
                throw new NotSupportedException("We could not process your audio with this model.");
            }

            if (isSpeakerDiarizationEnabled)
            {
                Console.WriteLine(results);

                // Speaker Tags are only included in the last result object, which has only one alternative.
                SpeechRecognitionAlternative alternative = results[results.Count - 1].Alternatives.FirstOrDefault();

                HandleSpeechRecognitionAlternative(alternative);
            }
            else
            {
                foreach (var result in results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        SaveTranscription(alternative.Transcript.Replace(". ", "." + Environment.NewLine));
                    }
                }
            }

        }

        private void HandleSpeechRecognitionAlternative(SpeechRecognitionAlternative alternative)
        {
            WordInfo firstWord = alternative.Words.FirstOrDefault();
            int currentSpeakerTag = firstWord.SpeakerTag;

            StringBuilder speakerWords = new StringBuilder($"Voice {firstWord.SpeakerTag}: {ModuleHelper.ConvertSecondsToMinutes(firstWord.StartTime.Seconds)} ");

            bool isEndedWithAFullStop = false;
            (speakerWords, isEndedWithAFullStop) = UpdateTranscriptStringBuilder(speakerWords, firstWord.Word, isEndedWithAFullStop);

            foreach (var newWord in alternative.Words.Skip(1).ToList())
            {
                if (newWord.SpeakerTag == currentSpeakerTag)
                {
                    (speakerWords, isEndedWithAFullStop) = UpdateTranscriptStringBuilder(speakerWords, newWord.Word, isEndedWithAFullStop);
                }
                else
                {
                    currentSpeakerTag = newWord.SpeakerTag;
                    if (!isEndedWithAFullStop)
                    {
                        speakerWords.AppendLine(string.Empty);
                    }
                    speakerWords = speakerWords.Append($"Voice {newWord.SpeakerTag}: {ModuleHelper.ConvertSecondsToMinutes(newWord.StartTime.Seconds)} ");

                    (speakerWords, isEndedWithAFullStop) = UpdateTranscriptStringBuilder(speakerWords, newWord.Word, false);

                }
            }
            if (speakerWords.Length > 0)
                SaveTranscription(speakerWords.ToString());
        }

        private static (StringBuilder speakerWords, bool isEndedWithAFullStop)
          UpdateTranscriptStringBuilder(StringBuilder stringBuilder, string newWord, bool isLastWordEndedWithAFullStop)
        {
            if (!newWord.EndsWith("."))
            {
                if (!isLastWordEndedWithAFullStop)
                {
                    stringBuilder.Append($" {newWord}");
                }
                else
                {
                    stringBuilder.Append($"{newWord}");
                }
                return (stringBuilder, false);
            }

            if (!isLastWordEndedWithAFullStop)
            {
                stringBuilder.AppendLine($" {newWord}");
            }
            else
            {
                stringBuilder.AppendLine($"{newWord}");
            }
            return (stringBuilder, true);
        }

        private RecognitionConfig GetRecognitionConfig(GoogeSpeechConfig googleSpeechConfig)
        {
            var config = new RecognitionConfig
            {
                LanguageCode = googleSpeechConfig.LanguageCode,
                Model = googleSpeechConfig.TranscriptionModel,
                EnableAutomaticPunctuation = googleSpeechConfig.EnableAutomaticPunctuation
            };

            config.AudioChannelCount = googleSpeechConfig.AudioChannelCount;

            if (googleSpeechConfig.EnableSeparateRecognitionPerChannel)
            {
                config.EnableSeparateRecognitionPerChannel = googleSpeechConfig.EnableSeparateRecognitionPerChannel;
            }
            else if (googleSpeechConfig.EnableSpeakerDiarization)
            {
                config.DiarizationConfig = new SpeakerDiarizationConfig()
                {
                    EnableSpeakerDiarization = googleSpeechConfig.EnableSpeakerDiarization//,
                    //MinSpeakerCount = googleSpeechConfig.DiarizationSpeakerCount,
                    //MaxSpeakerCount = googleSpeechConfig.DiarizationSpeakerCount
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
