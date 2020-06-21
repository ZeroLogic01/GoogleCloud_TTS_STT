using Google.Cloud.Speech.V1;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Google.Cloud.Speech.V1.RecognitionConfig.Types;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.Helpers
{
    internal class ModuleHelper
    {
        internal static List<TranscriptionModelModel> GetAllTranscriptionModels()
        {
            List<TranscriptionModelModel> models = new List<TranscriptionModelModel>()
            {
                new TranscriptionModelModel(){
                    ModelName="Default",
                    ModelValue="default"
                },
                new TranscriptionModelModel(){
                    ModelName="Phone Call",
                    ModelValue="phone_call"
                },
                new TranscriptionModelModel(){
                    ModelName="Video",
                    ModelValue="video"
                },
                new TranscriptionModelModel(){
                    ModelName="voice search",
                    ModelValue="command_and_search"
                }
            };


            return models.OrderBy(item => item.ModelName).ToList();
        }

        internal static List<SpeakerDiarization> GetSpeakerDiarizationList()
        {
            List<SpeakerDiarization> list = new List<SpeakerDiarization>()
            {
                new SpeakerDiarization(){
                    DisplayName="Off",
                    Value=SpeakerDiarizationEnum.Off
                },
                new SpeakerDiarization(){
                    DisplayName="Downmix and recognize multiple speakers",
                    Value=SpeakerDiarizationEnum.DownmixAndRecognizeMultipleSpeakers
                },
                new SpeakerDiarization(){
                    DisplayName="Recognize multiple speakers in single channel",
                    Value=SpeakerDiarizationEnum.RecognizeMultipleSpeakersInSingleChannel
                },
                new SpeakerDiarization(){
                    DisplayName="Recognize 1 speaker per channel",
                    Value=SpeakerDiarizationEnum.Recognize1SpeakerPerChannel
                },

            };
            return list;
        }

        internal static List<Speaker> GetSpeakersList()
        {
            List<Speaker> list = new List<Speaker>()
            {
                new Speaker(){
                    Name="2 Speakers",
                    Count=2
                },
                new Speaker(){
                    Name="3 Speakers",
                    Count=3
                },
                new Speaker(){
                    Name="4 Speakers",
                    Count=4
                },
                new Speaker(){
                    Name="5 Speakers",
                    Count=5
                },
                new Speaker(){
                    Name="6 Speaker",
                    Count=6
                }
            };
            return list;
        }


        //

        public static async Task TranscribeShortAudioFile(AudioEncoding encoding, int sampleRate, int channels, string localPath, string langCode,
           string transcriptionModel, string outputFilePath, bool enableSpeakerDiarization, int speakerCount, bool enableSeparateRecognitionPerChannel, CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                var speech = SpeechClient.Create();
                RecognizeResponse response = speech.Recognize(new RecognitionConfig()
                {
                    Encoding = encoding,
                    SampleRateHertz = sampleRate,
                    LanguageCode = langCode,
                    AudioChannelCount = channels,
                    EnableAutomaticPunctuation = true,
                    EnableWordTimeOffsets = true,
                    EnableSeparateRecognitionPerChannel = enableSeparateRecognitionPerChannel,
                    DiarizationConfig = enableSpeakerDiarization ? new SpeakerDiarizationConfig
                    {
                        EnableSpeakerDiarization = true,
                        MinSpeakerCount = speakerCount,
                        MaxSpeakerCount = speakerCount
                    } : null,
                    Model = transcriptionModel
                }, RecognitionAudio.FromFile(localPath));

                //await TextFileHelper.WriteTextAsync(outputFilePath, $"Word,StartTime,EndTime\n",
                //              cancellationToken);
                foreach (var result in response.Results)
                {
                    await HandleResult(result, outputFilePath, cancellationToken);
                }

            }, cancellationToken);
        }

        public static async Task TranscribeLongAudioFile(AudioEncoding encoding,
            int sampleRate, int channels, string storageUri, string langCode,
              string transcriptionModel, string outputFilePath, CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                var speech = SpeechClient.Create();
                var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
                {
                    Encoding = encoding,
                    SampleRateHertz = sampleRate,
                    AudioChannelCount = channels,
                    LanguageCode = langCode,
                    EnableAutomaticPunctuation = true,
                    EnableWordTimeOffsets = true,
                    Model = transcriptionModel
                }, RecognitionAudio.FromStorageUri(storageUri));
                longOperation = longOperation.PollUntilCompleted();

                var response = longOperation.Result;

                //await TextFileHelper.WriteTextAsync(outputFilePath, $"Word,StartTime,EndTime\n",
                //              cancellationToken);
                foreach (var result in response.Results)
                {
                    await HandleResult(result, outputFilePath, cancellationToken);
                }

            }, cancellationToken);
        }

        private static async Task HandleResult(SpeechRecognitionResult result, string outputFilePath, CancellationToken cancellationToken)
        {
            //List<WordTimeOffset> words = new List<WordTimeOffset>();
            //foreach (var alternative in result.Alternatives)
            //{
            //    Console.WriteLine($"Transcript: { alternative.Transcript}");

            //    foreach (var item in alternative.Words)
            //    {
            //        words.Add(new WordTimeOffset { Word = item.Word, StartTime = item.StartTime.ToString(), EndTime = item.EndTime.ToString() });
            //        //await TextFileHelper.WriteTextAsync(outputFilePath,
            //        //    $"{(item.Word.Contains(",") ? "/" + item.Word + "/" : item.Word)},{item.StartTime},{item.EndTime}\n",
            //        //    cancellationToken);
            //    }
            //    //Console.WriteLine(alternative.Transcript);
            //}
            //if (words.Count > 0)
            //{
            //    await TextFileHelper.WriteTextToCSVAsync(outputFilePath, words, cancellationToken);
            //}
        }
    }
}
