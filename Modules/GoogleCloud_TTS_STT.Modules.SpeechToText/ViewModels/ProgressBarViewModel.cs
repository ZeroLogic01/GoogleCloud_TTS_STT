using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.Enums;
using GoogleCloud_TTS_STT.Modules.SpeechToText.Core.EventAggregators;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace GoogleCloud_TTS_STT.Modules.SpeechToText.ViewModels
{
    public class ProgressBarViewModel : ViewModelBase
    {

        #region Progress Bar Properties

        private double _minimumValue;
        /// <summary>
        /// Progress Bar minimum value
        /// </summary>
        public double MinimumValue
        {
            get { return _minimumValue; }
            set { SetProperty(ref _minimumValue, value); }
        }

        private double _maximumValue;
        /// <summary>
        /// Progress Bar maximum value
        /// </summary>
        public double MaximumValue
        {
            get { return _maximumValue; }
            set { SetProperty(ref _maximumValue, value); }
        }

        private double _currentValue;
        /// <summary>
        /// Progress Bar current value
        /// </summary>
        public double CurrentValue
        {
            get { return _currentValue; }
            set { SetProperty(ref _currentValue, value); }
        }

        #endregion

        #region Class Constructors

        public ProgressBarViewModel() { }


        public ProgressBarViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ProgressBarEvent>().Subscribe(UpdateProgressBar, ThreadOption.PublisherThread, false);
        }

        #endregion

        #region Event Handlers

        private void UpdateProgressBar(ProgressBarEventParameters progress)
        {
            switch (progress.Type)
            {
                case ProgressType.ProgressBarCurrentValue:
                    CurrentValue = progress.Value;
                    break;
                case ProgressType.ProgressBarMaximumValue:
                    MaximumValue = progress.Value;
                    break;
                case ProgressType.ProgressBarMinimumValue:
                    MinimumValue = progress.Value;
                    break;
            }
        }

        #endregion

    }
}
