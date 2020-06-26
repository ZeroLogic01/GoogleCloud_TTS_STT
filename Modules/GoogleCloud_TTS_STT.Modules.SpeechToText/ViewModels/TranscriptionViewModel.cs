using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators;
using Prism.Commands;
using Prism.Events;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.ViewModels
{
    public class TranscriptionViewModel : ViewModelBase
    {
        #region Binded Properties

        private string _transcribedText;
        public string TranscribedText
        {
            get { return _transcribedText; }
            set
            {
                SetProperty(ref _transcribedText, value);
                SaveAsCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands
        public DelegateCommand SaveAsCommand { get; set; }

        #endregion

        #region Class Constructors

        public TranscriptionViewModel() { }
        public TranscriptionViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TranscriptEvent>().Subscribe(UpdateTranscription, ThreadOption.PublisherThread, false);
            SaveAsCommand = new DelegateCommand(SaveAs, CanSaveAs);
        }


        #endregion

        #region Save Command Methods

        private void SaveAs()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Text file|*.txt|JSON File|*.json|All Files|*.*";
                dialog.Title = "Save the transcription to a file";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dialog.FileName, TranscribedText, Encoding.UTF8);
                }

            }
        }

        private bool CanSaveAs()
        {
            return !string.IsNullOrWhiteSpace(_transcribedText);
        }
        
        #endregion

        #region Methods

        private void UpdateTranscription(string transcript)
        {
            var padding = string.IsNullOrWhiteSpace(TranscribedText) ? string.Empty : " ";
            TranscribedText += $"{padding}{transcript}";
        }

        #endregion
    }
}
