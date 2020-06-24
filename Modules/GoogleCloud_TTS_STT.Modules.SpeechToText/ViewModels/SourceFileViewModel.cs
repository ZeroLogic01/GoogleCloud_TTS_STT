using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Views;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.ViewModels
{
    class SourceFileViewModel : BindableBase
    {
        #region Properties

        #region Binded Properties

        private string _sourceFile;
        public string SourceFile
        {
            get { return _sourceFile; }
            set
            {
                SetProperty(ref _sourceFile, value);
                TranscribeCommand.RaiseCanExecuteChanged();
            }
        }

        private Uri _sourceFileURI;
        public Uri SourceFileURI
        {
            get { return _sourceFileURI; }
            set
            {
                SetProperty(ref _sourceFileURI, value);
                TranscribeCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isTranscriptionJobRunning = false;
        public bool IsTranscriptionJobRunning
        {
            get { return _isTranscriptionJobRunning; }
            set { SetProperty(ref _isTranscriptionJobRunning, value); }
        }

        #region Speech to text button

        private bool _isTranscribeButtonEnabled = false;
        public bool IsTranscribeButtonEnabled
        {
            get { return _isTranscribeButtonEnabled; }
            set
            {
                SetProperty(ref _isTranscribeButtonEnabled, value);
                CancelSpeechToTextCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Cancel Speech To Text Button

        private bool _isCancelButtonEnabled = false;
        public bool IsCancelButtonEnabled
        {
            get { return _isCancelButtonEnabled; }
            set
            {
                SetProperty(ref _isCancelButtonEnabled, value);
            }
        }

        #endregion

        #region File to choose from

        private bool _isLocalStorageEnabled = true;
        public bool IsLocalStorageEnabled
        {
            get { return _isLocalStorageEnabled; }
            set
            {
                SetProperty(ref _isLocalStorageEnabled, value);
                IsCloudStorageEnabled = !value;

                TranscribeCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isCloudStorageEnabled;
        public bool IsCloudStorageEnabled
        {
            get { return _isCloudStorageEnabled; }
            set { SetProperty(ref _isCloudStorageEnabled, value); }
        }


        #endregion


        #region Navigate Command Button

        private Visibility _isNavigateCommandVisisble = Visibility.Visible;
        public Visibility IsNavigateCommandVisisble
        {
            get { return _isNavigateCommandVisisble; }
            set { SetProperty(ref _isNavigateCommandVisisble, value); }
        }

        #endregion

        #endregion

        #region Private Properties

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _ea;
        private CancellationTokenSource _cts;

        #endregion

        #region Commands


        /// <summary>
        /// The command to control speech to text
        /// </summary>
        public DelegateCommand TranscribeCommand { get; set; }

        /// <summary>
        /// The command to stop (cancel) speech to text
        /// </summary>
        public DelegateCommand CancelSpeechToTextCommand { get; set; }

        public DelegateCommand ChooseFileCommand { get; set; }
        public DelegateCommand NavigateCommand { get; set; }

        #endregion

        #endregion


        public SourceFileViewModel(IRegionManager regionManager, IEventAggregator ea)
        {
            _regionManager = regionManager;
            _ea = ea;
            _cts = new CancellationTokenSource();

            IsCloudStorageEnabled = !IsLocalStorageEnabled;

            ChooseFileCommand = new DelegateCommand(ChooseSourceFile);
            NavigateCommand = new DelegateCommand(ActivateTranscriptionSettingsView);
            TranscribeCommand = new DelegateCommand(PerformSpeechToText, CanPerformSpeechToText);
            CancelSpeechToTextCommand = new DelegateCommand(CancelSpeechToText, CanCancelSpeechToText);
        }

        private void ChooseSourceFile()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = @"Custom Files(*.opus|*.flac|*.webm|*.weba)",
                //*.oga;*.mid;*.mp3;*.aiff;*.wma;*.au;*.ogm;*.wmv;*.mpg;*.webm;*.ogv;*.mov;*.asx;*.mpeg;*.mp4;*.m4v;*.avi)",
                Multiselect = false,
                DefaultExt = ".flac"
            };

            if (dialog.ShowDialog() == true)
            {
                SourceFile = dialog.FileName;
            }

        }

        #region Methods

        private void ActivateTranscriptionSettingsView()
        {
            _regionManager.RequestNavigate(Core.Regions.RegionNames.SpeechToTextAPIConfigRegion, nameof(TranscriptionSettingsView));
        }


        #region STT Command Methods

        private async void PerformSpeechToText()
        {
            IsTranscriptionJobRunning = true;
            IsNavigateCommandVisisble = Visibility.Collapsed;
            TranscribeCommand.RaiseCanExecuteChanged();
            CancelSpeechToTextCommand.RaiseCanExecuteChanged();

            try
            {
                await UpdateStatus("Performing Speech to Text...");

                await Task.Delay(3000, _cts.Token);

                await UpdateStatus("Transcription completed", 1);
                await UpdateStatus("");
            }
            catch (TaskCanceledException) { }
            catch (Exception e)
            {
                await AppHelper.ShowMessage("Error", ExceptionHelper.ExtractExceptionMessage(e));
            }
            finally
            {
                IsTranscriptionJobRunning = false;
                IsNavigateCommandVisisble = Visibility.Visible;
                TranscribeCommand.RaiseCanExecuteChanged();
                CancelSpeechToTextCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanPerformSpeechToText()
        {
            if (IsLocalStorageEnabled)
            {
                IsTranscribeButtonEnabled = !string.IsNullOrWhiteSpace(SourceFile) && !IsTranscriptionJobRunning;
            }
            else
            {
                IsTranscribeButtonEnabled = !string.IsNullOrWhiteSpace(SourceFileURI.ToString()) && !IsTranscriptionJobRunning;
            }
            return IsTranscribeButtonEnabled;
        }

        #endregion

        #region Cancel Speech To Text Command Methods


        private async void CancelSpeechToText()
        {
            try
            {
                IsCancelButtonEnabled = false;

                CancelSpeechToTextCommand.RaiseCanExecuteChanged();


                if (!_cts.IsCancellationRequested)
                {
                    _cts.Cancel(true);
                }
            }
            catch (Exception) { }
            finally
            {
                _cts?.Dispose(); // Clean up old token source....
                _cts = new CancellationTokenSource(); // "Reset" the cancellation token source...

                IsNavigateCommandVisisble = Visibility.Visible;
                IsTranscriptionJobRunning = false;
            }
        }
        private bool CanCancelSpeechToText()
        {
            return IsCancelButtonEnabled = !IsTranscribeButtonEnabled && IsTranscriptionJobRunning;
        }

        #endregion

        private async Task UpdateStatus(string message, int delayInSeconds = 0)
        {
            _ea.GetEvent<StatusEvent>().Publish(
                new StatusEventParameters
                {
                    Message = message
                });
            await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
        }

        #endregion

    }
}
