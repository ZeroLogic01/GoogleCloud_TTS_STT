using GoogleCloud_TTS_STT.Core;
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
    public partial class TextToSpeechView : UserControl
    {

        public TextToSpeechView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            eventAggregator.GetEvent<SsmlBreakEvent>().Subscribe(AddSsmlBreak, ThreadOption.PublisherThread, false);
        }

        private async void AddSsmlBreak(string tagContent)
        {
            if (TextBox_SSML.Text.Length + tagContent.Length <= AppConstants.MaximumNumberOfSsmlCharactersAllowed_TTS)
            {
                if (TextBox_SSML.Text.Length > 0 && TextBox_SSML.CaretIndex > 0)
                {
                    TextBox_SSML.Text = TextBox_SSML.Text.Insert(TextBox_SSML.CaretIndex, $" {tagContent}");
                    TextBox_SSML.Text = TextBox_SSML.Text.Replace("  ", " ");
                }
                else
                {
                    TextBox_SSML.Text = TextBox_SSML.Text.Insert(TextBox_SSML.CaretIndex, tagContent);
                }
            }
            else
            {
                await AppHelper.ShowMessage("Maximum length will be exceeded!", "Cannot insert a new Break tag because the length of remaining" +
                    " characters allowed is less than the number of characters required for Break tag.");
            }
        }

        //private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //await ((MetroWindow)Window.GetWindow(this)).ShowChildWindowAsync<bool>(new BreakTag() { IsModal = true, AllowMove = true }, ChildWindowManager.OverlayFillBehavior.WindowContent);
        //}
    }
}
