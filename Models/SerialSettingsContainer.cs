using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Xml.Serialization;
using static AvaloniaToolbox.Models.SerialConstants;

namespace AvaloniaToolbox.Models
{
    /// <summary>
    /// Container for all values needed to instantiate connections via serial ports and sockets
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "SerialSettingsContainer")]
    public class SerialSettingsContainer : BasePropertyChanged
    {
        public SerialSettingsContainer()
        {
            
        }

        public SerialSettingsContainer(
            string serialPort,
            bool stabilityIndicatorActive,
            bool sequenceOfIdenticalReadingsActive,
            int? baudRate = null,
            int? databits = null,
            int? stopBits = null,
            string? parity = null,
            string? flowControl = null,
            string? stabilityIndicatorSnippet = null,
            int? stabilityIndicatorStartingPosition = null,
            int? numberOfIdenticalReadings = null,
            int? scaleStringWeightStartingPosition = null,
            int? scaleStringWeightEndingPosition = null,
            int? scaleStringMinimumLength = null,
            int? broadcastPort = null,
            int? serialTimeoutMs = null
        )

        {
            SerialPort = serialPort;

            if (broadcastPort is not null)
                BroadcastPort = (int)broadcastPort;

            if (baudRate is not null)
                BaudRate = (int)baudRate;

            if (databits is not null)
                Databits = (int)databits;

            if (flowControl is not null)
                FlowControl = flowControl;

            if (numberOfIdenticalReadings is not null)
                NumberOfIdenticalReadings = (int)numberOfIdenticalReadings;

            if (parity is not null)
                Parity = parity;

            ReceivedValues = new();

            if (scaleStringMinimumLength is not null)
                ScaleStringRequiredLength = (int)scaleStringMinimumLength;
 
            if (scaleStringWeightStartingPosition is not null)
                ScaleStringWeightStartingPosition = (int)scaleStringWeightStartingPosition;

            if (scaleStringWeightEndingPosition is not null)
                ScaleStringWeightEndingPosition = (int)scaleStringWeightEndingPosition;

            if (serialTimeoutMs is not null)
                SerialTimeoutMs = (int)serialTimeoutMs;

            if (stabilityIndicatorSnippet is not null)
                StabilityIndicatorSnippet = stabilityIndicatorSnippet;

            if (stabilityIndicatorStartingPosition is not null)
                StabilityIndicatorStartingPosition = (int)stabilityIndicatorStartingPosition;

            if (stopBits is not null)
                StopBits = (int)stopBits;

            StabilityIndicatorActive            = stabilityIndicatorActive;
            SequenceOfIdenticalReadingsActive   = sequenceOfIdenticalReadingsActive;

            PropertyChangedHandler += SerialSettingsContainer_PropertyChanged;
        }

        public virtual void SerialSettingsContainer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        private int? _baudRate;

        [XmlElement]
        public int? BaudRate
        {
            get => _baudRate;
            set => SetProperty(ref _baudRate, value);
        }

        private int _broadcastPort = -1;

        [XmlElement]
        public int BroadcastPort
        {
            get => _broadcastPort;
            set => SetProperty(ref _broadcastPort, (value <= 0) ? 5050 : value);
        }

        private int? _databits;

        [XmlElement]
        public int? Databits
        {
            get => _databits;
            set => SetProperty(ref _databits, value);
        }

        private string? _flowControl;

        [XmlElement]
        public string? FlowControl
        {
            get => _flowControl;
            set => SetProperty(ref _flowControl, value);
        }

        [XmlIgnore]
        public FlowControl flowControl => FlowControl switch
        {
            FlowControlCtsRts => SerialConstants.FlowControl.Ctr_Rts,
            FlowControlDsrDtr => SerialConstants.FlowControl.Dsr_Dtr,
            FlowControlNone => SerialConstants.FlowControl.None,
            FlowControlXonXoff => SerialConstants.FlowControl.Xon_Xoff,
            _ => SerialConstants.FlowControl.None,
        };

        internal void SetFlowControl(FlowControl? flowControl)
        {
            FlowControl = flowControl switch
            {
                SerialConstants.FlowControl.Ctr_Rts  => FlowControlCtsRts,
                SerialConstants.FlowControl.Dsr_Dtr  => FlowControlDsrDtr,
                SerialConstants.FlowControl.Xon_Xoff => FlowControlXonXoff,
                SerialConstants.FlowControl.None     => FlowControlNone,
                _ => FlowControlNone,
            };
        }

        public string GetFlowControl()
        {
            return flowControl switch
            {
                SerialConstants.FlowControl.Ctr_Rts  => FlowControlCtsRts,
                SerialConstants.FlowControl.Dsr_Dtr  => FlowControlDsrDtr,
                SerialConstants.FlowControl.Xon_Xoff => FlowControlXonXoff,
                SerialConstants.FlowControl.None     => FlowControlNone,
                _ => FlowControlNone,
            };
        }

        private int _numberOfIdenticalReadings;

        [XmlElement]
        public int NumberOfIdenticalReadings
        {
            get => _numberOfIdenticalReadings;
            set => SetProperty(ref _numberOfIdenticalReadings, (value < 1) ? 1 : value);
        }

        private string? _parity;

        [XmlElement]
        public string? Parity
        {
            get => _parity;
            set => SetProperty(ref _parity, value);
        }

        private List<string> _receivedValues = new();

        [XmlElement]
        public List<string> ReceivedValues
        {
            get => _receivedValues;
            set => SetProperty(ref _receivedValues, value);
        }

        private int? _scaleStringRequiredLength;

        [XmlElement]
        public int? ScaleStringRequiredLength
        {
            get => _scaleStringRequiredLength;
            set => SetProperty(ref _scaleStringRequiredLength, (value < 0) ? 0 : value, () =>
            {
                OnPropertyChanged(nameof(ScaleStringMustConformToLength));
            });
        }

        public bool ScaleStringMustConformToLength => ScaleStringRequiredLength > 0;

        private int? _scaleStringWeightEndingPosition;

        [XmlElement]
        public int? ScaleStringWeightEndingPosition
        {
            get => _scaleStringWeightEndingPosition;
            set => SetProperty(ref _scaleStringWeightEndingPosition, (ScaleStringWeightStartingPosition > 0 && ScaleStringWeightStartingPosition > value)
                ? ScaleStringWeightStartingPosition
                : (value < 0)
                    ? 1
                    : value);
        }

        private int? _scaleStringWeightStartingPosition;

        [XmlElement]
        public int? ScaleStringWeightStartingPosition
        {
            get => _scaleStringWeightStartingPosition;
            set => SetProperty(ref _scaleStringWeightStartingPosition, (ScaleStringWeightEndingPosition > 0 && value > ScaleStringWeightEndingPosition)
                ? ScaleStringWeightEndingPosition
                : (value < 0)
                    ? 1
                    : value);
        }

        private string _serialPort = string.Empty;

        [XmlElement]
        public string SerialPort
        {
            get => _serialPort;
            set
            {
                if (string.IsNullOrWhiteSpace(_serialPort))
                {
                    _serialPort = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _sequenceOfIdenticalReadingsActive = false;

        [XmlElement]
        public bool SequenceOfIdenticalReadingsActive
        {
            get => _sequenceOfIdenticalReadingsActive;
            set => SetProperty(ref _sequenceOfIdenticalReadingsActive, value);
        }

        private int? _serialTimeoutMs;

        [XmlElement]
        public int? SerialTimeoutMs
        {
            get => _serialTimeoutMs;
            set => SetProperty(ref _serialTimeoutMs, value ?? 500);
        }

        internal void AddToReceivedValues(string lastReceivedValue)
        {
            if (SequenceOfIdenticalReadingsActive && NumberOfIdenticalReadings > 0)
            {
                if (ReceivedValues?.Count == NumberOfIdenticalReadings == true)
                {
                    ReceivedValues.RemoveAt(ReceivedValues.Count - 1);
                }
                else if ((ReceivedValues?.Count > NumberOfIdenticalReadings) == true)
                {
                    int difference = ReceivedValues.Count - NumberOfIdenticalReadings;
                    for (int i = 0; i < difference; i++)
                    {
                        ReceivedValues.RemoveAt(ReceivedValues.Count - 1);
                    }
                }

                if (ReceivedValues == null)
                {
                    ReceivedValues = new List<string>()
                    {
                        lastReceivedValue
                    };
                }
                else
                {
                    ReceivedValues.Insert(0, lastReceivedValue);
                }
            }
        }

        private bool _stabilityIndicatorActive = false;

        [XmlElement]
        public bool StabilityIndicatorActive
        {
            get => _stabilityIndicatorActive;
            set => SetProperty(ref _stabilityIndicatorActive, value);
        }

        internal void SetParity(Parity? parity)
        {
            Parity = parity switch
            {
                System.IO.Ports.Parity.None     => NoParity,
                System.IO.Ports.Parity.Odd      => OddParity,
                System.IO.Ports.Parity.Even     => EvenParity,
                _ => NoParity,
            };
        }

        internal Parity parity => Parity switch
        {
            OddParity => System.IO.Ports.Parity.Odd,
            EvenParity => System.IO.Ports.Parity.Even,
            NoParity => System.IO.Ports.Parity.None,
            _ => System.IO.Ports.Parity.None
        };

        public string GetParity()
        {
            return parity switch
            {
                System.IO.Ports.Parity.None     => NoParity,
                System.IO.Ports.Parity.Odd      => OddParity,
                System.IO.Ports.Parity.Even     => EvenParity,
                _ => NoParity,
            };
        }

        private string? _stabilityIndicatorSnippet;

        [XmlElement]
        public string? StabilityIndicatorSnippet
        {
            get => _stabilityIndicatorSnippet;
            set => SetProperty(ref _stabilityIndicatorSnippet, value);
        }

        private int _stabilityIndicatorStartingPosition;

        [XmlElement]
        public int StabilityIndicatorStartingPosition
        {
            get => _stabilityIndicatorStartingPosition;
            set => SetProperty(ref _stabilityIndicatorStartingPosition, (value < 0) ? 1 : value);
        }

        private int? _stopBits;

        [XmlElement]
        public int? StopBits
        {
            get => _stopBits;
            set => SetProperty(ref _stopBits, value);
        }
    }
}
