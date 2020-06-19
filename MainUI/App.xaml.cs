using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText;
using GoogleCloud_TTS_STT.Modules.TextToSpeech;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MainUI.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace MainUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<TextToSpeechModule>();
            moduleCatalog.AddModule<SpeechToTextModule>();

        }

        private async void PrismApplication_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var message = ExceptionHelper.ExtractExceptionMessage(e.Exception);
            await DialogManager.ShowMessageAsync(Current.MainWindow as MetroWindow, "An unknown error occurred!",
                message);

            e.Handled = true;
        }
    }
}
