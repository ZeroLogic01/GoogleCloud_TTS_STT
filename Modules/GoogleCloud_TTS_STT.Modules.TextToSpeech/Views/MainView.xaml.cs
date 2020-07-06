using GoogleCloud_TTS_STT.Modules.TextToSpeech.SSML;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            await Application.Current.MainWindow.ShowChildWindowAsync<bool>(new AddBreak() { IsModal = true, AllowMove = true }, ChildWindowManager.OverlayFillBehavior.FullWindow);

        }
    }
}
