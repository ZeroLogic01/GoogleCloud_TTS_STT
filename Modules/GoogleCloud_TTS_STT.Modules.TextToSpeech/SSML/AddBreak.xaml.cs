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

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.SSML
{
    /// <summary>
    /// Interaction logic for AddBreak.xaml
    /// </summary>
    public partial class AddBreak : ChildWindow
    {
        public AddBreak()
        {
            InitializeComponent();
        }

        private void OkButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.Close(true);
        }

        private void CloseSec_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close(false);
        }
    }
}
