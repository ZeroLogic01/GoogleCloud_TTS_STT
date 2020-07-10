using GoogleCloud_TTS_STT.Core;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.Enums;
using GoogleCloud_TTS_STT.Modules.TextToSpeech.EventAggregators;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoogleCloud_TTS_STT.Modules.TextToSpeech.ViewModels
{
    public class BreakTagViewModel : BindableBase
    {
        #region Properties

        #region Attributes

        private List<string> _attributes;
        public List<string> Attributes
        {
            get { return _attributes; }
            set { SetProperty(ref _attributes, value); }
        }

        private string _selectedAttribute;
        public string SelectedAttribute
        {
            get { return _selectedAttribute; }
            set
            {
                SetProperty(ref _selectedAttribute, value);
            }
        }

        #endregion

        #region Time Attribute

        private List<string> _timeAttributeTypes;
        public List<string> TimeAttributeTypes
        {
            get { return _timeAttributeTypes; }
            set { SetProperty(ref _timeAttributeTypes, value); }
        }

        private string _selectedTimeAttributeTypes;
        public string SelectedTimeAttributeTypes
        {
            get { return _selectedTimeAttributeTypes; }
            set { SetProperty(ref _selectedTimeAttributeTypes, value); }
        }

        #endregion


        #region Strength Attribute

        private List<string> _strengths = GetAvailableStrengths();
        public List<string> Strengths
        {
            get { return _strengths; }
            set { SetProperty(ref _strengths, value); }
        }

        private string _selectedStrength;
        public string SelectedStrength
        {
            get { return _selectedStrength; }
            set
            {
                SetProperty(ref _selectedStrength, value);
            }
        }

        #endregion


        private double _length = 1;
        public double Length
        {
            get { return _length; }
            set
            {
                SetProperty(ref _length, value);
            }
        }

        private string BreakTagContent
        {
            get
            {
                Enum.TryParse(SelectedAttribute, out BreakTagAttribute breakTagAttribute);
                string content = breakTagAttribute == BreakTagAttribute.Time
                    ? $"time=\"{Length}{SelectedTimeAttributeTypes}\""
                    : $"strength=\"{SelectedStrength}\"";
               
                return $"<break {content}/>";
            }
        }

        #endregion

        #region Commands

        private bool _saveBeforeClosing;
        public bool SaveBeforeClosing
        {
            get { return _saveBeforeClosing; }
            set
            {
                SetProperty(ref _saveBeforeClosing, value);
            }
        }


        private DelegateCommand<object> _closeButtonCommand;
        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<object> CloseCommand
        {
            get { return _closeButtonCommand; }
            set
            {
                SetProperty(ref _closeButtonCommand, value);
            }
        }


        #endregion


        #region Constructor

        public BreakTagViewModel()
        {

        }

        public BreakTagViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            CloseCommand = new DelegateCommand<object>(Execute);

            Attributes = Enum.GetNames(typeof(BreakTagAttribute)).ToList();
            SelectedAttribute = BreakTagAttribute.Time.ToString();

            TimeAttributeTypes = Enum.GetNames(typeof(TimeAttributeType)).ToList();
            SelectedTimeAttributeTypes = TimeAttributeType.s.ToString();
            SelectedStrength = Strengths.FirstOrDefault();
        }

        private void Execute(object obj)
        {
            if (obj is bool saveBeforeClosing && saveBeforeClosing)
            {
               // Console.WriteLine(BreakTagContent);
                _eventAggregator.GetEvent<SsmlBreakEvent>().Publish(BreakTagContent);
            }
        }


        #endregion


        /// <summary>
        ///  the strength of the output's prosodic break by relative terms. Valid values are: "x-weak", weak", "medium", 
        ///  "strong", and "x-strong". The value "none" indicates that no prosodic break boundary should be outputted, 
        ///  which can be used to prevent a prosodic break that the processor would otherwise produce. 
        ///  The other values indicate monotonically non-decreasing (conceptually increasing) break strength between tokens.
        ///  The stronger boundaries are typically accompanied by pauses.
        /// </summary>
        /// <returns></returns>
        private static List<string> GetAvailableStrengths()
        {
            return new List<string>()
            {
                "none",
                "x-weak",
                "weak",
                "medium",
                "strong",
                "x-strong"
            };
        }
    }
}
