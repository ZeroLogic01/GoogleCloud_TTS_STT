using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech
{
    public class TextToSpeechModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.Regions[RegionNames.TextToSpeechRegion].Add(containerProvider.Resolve<MainView>());

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}