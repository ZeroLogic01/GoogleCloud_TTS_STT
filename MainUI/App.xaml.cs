using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText;
using GoogleCloud_TTS_STT.Modules.TextToSpeech;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MainUI.Core.Regions;
using MainUI.ViewModels;
using MainUI.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.IO;
using System.Windows;

namespace MainUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            var jsonFile = Environment.GetEnvironmentVariable(AppConstants.GOOGLE_APPLICATION_CREDENTIALS);
            if (!File.Exists(jsonFile))
            {
                MainWindowViewModel.SetEnvironmentVariable();
                App.Current.Shutdown();
            }
        }
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<TextToSpeechModule>();
            moduleCatalog.AddModule<SpeechToTextModule>();

        }

        private async void PrismApplication_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var errorMsg = ExceptionHelper.ExtractExceptionMessage(e.Exception);
            {
                string caption = "An unknown error occurred!";
                try
                {
                    await DialogManager.ShowMessageAsync(Current.MainWindow as MetroWindow, caption,
                        errorMsg);
                }
                catch { MessageBox.Show(errorMsg, caption, MessageBoxButton.OK, MessageBoxImage.Error); }
            }

            e.Handled = true;
        }


        protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            base.ConfigureDefaultRegionBehaviors(regionBehaviors);

            regionBehaviors.AddIfMissing(DependentViewRegionBehavior.BehaviorKey, typeof(DependentViewRegionBehavior));
        }
    }
}
