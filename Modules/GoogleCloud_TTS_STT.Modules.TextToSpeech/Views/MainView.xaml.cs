using GoogleCloud_TTS_STT.Modules.TextToSpeech.EventAggregators;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.SSML;
using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using Prism.Events;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class MainView : UserControl
    {

        public MainView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            eventAggregator.GetEvent<SsmlBreakEvent>().Subscribe(AddSsmlBreak, ThreadOption.PublisherThread, false);


        }

        private void AddSsmlBreak(string tagContent)
        {
            if (TextBox_TTS.Text.Length > 0)
            {
                TextBox_TTS.Text = TextBox_TTS.Text.Insert(TextBox_TTS.CaretIndex, $" {tagContent} ");
                TextBox_TTS.Text = TextBox_TTS.Text.Replace("  ", " ");
            }
            else
            {
                TextBox_TTS.Text = $"{tagContent} ";
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //await ((MetroWindow)Window.GetWindow(this)).ShowChildWindowAsync<bool>(new BreakTag() { IsModal = true, AllowMove = true }, ChildWindowManager.OverlayFillBehavior.WindowContent);
        }
    }
}
