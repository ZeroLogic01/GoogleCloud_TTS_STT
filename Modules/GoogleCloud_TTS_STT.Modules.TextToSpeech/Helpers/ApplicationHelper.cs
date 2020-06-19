using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.Helpers
{
    public static class ApplicationHelper
    {
        /// <summary>
        /// Simple method which can be used on a Button
        /// </summary>
        public static async Task ShowMessage(string title, string message, MessageDialogStyle dialogStyle = MessageDialogStyle.Affirmative)
        {
            await DialogManager.ShowMessageAsync(Application.Current.MainWindow as MetroWindow, title, message, dialogStyle);
        }
    }
}
