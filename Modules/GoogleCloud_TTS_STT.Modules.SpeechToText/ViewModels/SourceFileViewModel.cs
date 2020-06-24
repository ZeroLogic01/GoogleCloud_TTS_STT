using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Views;
using Microsoft.Win32;
using Prism.Commands;
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

                IsTranscribeButtonEnabled = !string.IsNullOrWhiteSpace(value);
                TranscribeCommand.RaiseCanExecuteChanged();
            }
        }

        #region Speech to text button

        private bool _isTranscribeButtonEnabled = false;
        public bool IsTranscribeButtonEnabled
        {
            get { return _isTranscribeButtonEnabled; }
            set
            {
                SetProperty(ref _isTranscribeButtonEnabled, value);

                IsCancelButtonEnabled = !_isTranscribeButtonEnabled;
                IsCancelButtonVisisble = _isTranscribeButtonEnabled ? Visibility.Collapsed : Visibility.Visible;
                CancelSpeechToTextCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isLocalStorage = true;

        public bool IsLocalStorage
        {
            get { return _isLocalStorage; }
            set { SetProperty(ref _isLocalStorage, value); }
        }


        private Visibility _isTranscribeButtonVisible = Visibility.Collapsed;
        public Visibility IsTranscribeButtonVisible
        {
            get { return _isTranscribeButtonVisible; }
            set { SetProperty(ref _isTranscribeButtonVisible, value); }
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

        private Visibility _isCancelButtonVisisble = Visibility.Collapsed;
        public Visibility IsCancelButtonVisisble
        {
            get { return _isCancelButtonVisisble; }
            set { SetProperty(ref _isCancelButtonVisisble, value); }
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


        public SourceFileViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _cts = new CancellationTokenSource();

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
            IsTranscribeButtonEnabled = false;
            IsNavigateCommandVisisble = Visibility.Collapsed;
            TranscribeCommand.RaiseCanExecuteChanged();


            try
            {
                Console.WriteLine("Performing Speech to Text (Message)");





                await Task.Delay(10000, _cts.Token);
            }
            catch (TaskCanceledException) { }
            catch (Exception e)
            {
                await AppHelper.ShowMessage("Error", ExceptionHelper.ExtractExceptionMessage(e));
            }

            IsTranscribeButtonEnabled = true;
            TranscribeCommand.RaiseCanExecuteChanged();
        }

        private bool CanPerformSpeechToText()
        {
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
                IsCancelButtonVisisble = Visibility.Collapsed;
            }
        }
        private bool CanCancelSpeechToText()
        {
            return !IsTranscribeButtonEnabled;
        }

        #endregion

        #endregion

    }
}
