using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText
{
    public class SpeechToTextModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.Regions[RegionNames.SpeechToTextRegion].Add(containerProvider.Resolve<SpeechToTextView>());

            //Regions within module
            regionManager.Regions[Core.Regions.RegionNames.SpeechToTextAPIConfigRegion]
                .Add(containerProvider.Resolve<TranscriptionSettingsView>());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TranscriptionSettingsView>();
            containerRegistry.RegisterForNavigation<SourceFileView>();
        }
    }
}