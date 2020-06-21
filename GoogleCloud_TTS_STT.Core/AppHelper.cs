using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleCloud_TTS_STT.Core
{
    public static class AppHelper
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Simple method which can be used on a Button
        /// </summary>
        public static async Task ShowMessage(string title, string message, MessageDialogStyle dialogStyle = MessageDialogStyle.Affirmative)
        {
            await DialogManager.ShowMessageAsync(Application.Current.MainWindow as MetroWindow, title, message, dialogStyle);
        }
    }
}
