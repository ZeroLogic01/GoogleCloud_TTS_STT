using GoogleCloud_TTS_STT.Core;
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
            
            
            _regionManager.RegisterViewWithRegion(Core.Regions.RegionNames.SpeechToTextAPIConfigRegion, typeof(TranscriptionSettingsView));

            //Regions within module
            //_regionManager.Regions[Core.Regions.RegionNames.SpeechToTextAPIConfigRegion]
            //    .Add(containerProvider.Resolve<TranscriptionSettingsView>());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TranscriptionSettingsView>();
            containerRegistry.RegisterForNavigation<SourceFileView>();
        }

    }
}