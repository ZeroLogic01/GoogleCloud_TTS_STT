using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Interfaces;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Business.Models;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText
{
    public class SpeechToTextModule : IModule
    {

        private readonly IRegionManager _regionManager;

        public SpeechToTextModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.SpeechToTextRegion, typeof(SpeechToTextView));

            _regionManager.RegisterViewWithRegion(Core.Regions.RegionNames.TranscriptionRegion, typeof(Transcription));
            _regionManager.RegisterViewWithRegion(Core.Regions.RegionNames.ProgessBarRegion, typeof(ProgressBar));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterForNavigation<TranscriptionSettingsView>();
            //containerRegistry.RegisterForNavigation<SourceFileView>();

            containerRegistry.Register<ISpeechConfig, GoogeSpeechConfig>();
            containerRegistry.RegisterSingleton<ICloudStorage, GoogleCloudStorage>();
            containerRegistry.RegisterSingleton<ITranscriber, GoogleTranscriber>();
        }

    }
}