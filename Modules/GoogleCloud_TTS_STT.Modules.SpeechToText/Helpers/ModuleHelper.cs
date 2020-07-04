using Google.Cloud.Speech.V1;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business;
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
                    Name="Auto Detect",
                    Count=-1
                },
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

        public static string ConvertSecondsToMinutes(double seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);
            string hours = time.Hours > 0 ? string.Format("{0:D2}:", time.Hours) : string.Empty;
            return $"{hours}{time.Minutes}:{string.Format("{0:D2}", time.Seconds)}";
        }

    }
}
