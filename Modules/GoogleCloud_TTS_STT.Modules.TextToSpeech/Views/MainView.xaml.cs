using GoogleCloud_TTS_STT.Modules.TextToSpeech.SSML;
using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using System.Windows.Controls;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class MainView : UserControl
    {

        public MainView()
        {
            InitializeComponent();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await ((MetroWindow)Window.GetWindow(this)).ShowChildWindowAsync<bool>(new AddBreak() { IsModal = true, AllowMove = true }, ChildWindowManager.OverlayFillBehavior.WindowContent);

        }
    }
}
