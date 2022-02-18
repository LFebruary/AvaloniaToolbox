using Avalonia.Threading;
using System.Globalization;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using AvaloniaToolbox.UI.CustomIcons;
using static AvaloniaToolbox.Models.SerialConstants;
using AvaloniaToolbox.Functions;
using AvaloniaToolbox.UI;
using QRCoder;

namespace AvaloniaToolbox.Models
{
    /// <summary>
    /// Class to wrap around container of serial port settings and add additional methods/properties to establish socket connections <br/>
    /// And retrieve values from associated serial port.
    /// </summary>
    public class SerialWrapper : BasePropertyChanged, IDisposable
    {
        private string _lastProcessedValue = string.Empty;
        /// <summary>
        /// Last reading from serial port that has been processed.
        /// </summary>
        public string LastProcessedValue
        {
            get => _lastProcessedValue;
            set => SetProperty(ref _lastProcessedValue, value);
        }

        /// <summary>
        /// Whether socket button should be enabled or not
        /// </summary>
        public bool SocketButtonEnabled => ReadSwitch && (string.IsNullOrWhiteSpace(LastSerialReading) == false);

        /// <summary>
        /// Whether socket-parameter controls should be enabled or not
        /// </summary>
        public bool SocketControlsEnabled => ListeningFlag == false;

        public CustomIconType BroadcastButtonIcon => ReadSwitch == false || ListeningFlag == false
                    ? CustomIconType.BroadcastOff
                    : CustomIconType.Broadcast;

        public bool Active => ReadSwitch == false || ListeningFlag == false;

        public SerialWrapper(SerialSettingsContainer container, Action<int>? removeCallback = null)
        {
            _container = container;

            if (removeCallback is not null)
            {
                _removeCallback = removeCallback;
            }

            BroadcastPort = _container.BroadcastPort;

            _container.PropertyChangedHandler += (_, _) =>
            {
                OnPropertyChanged(nameof(LastUsedBaudRate));
                OnPropertyChanged(nameof(LastUsedDataBits));
                OnPropertyChanged(nameof(LastUsedParity));
                OnPropertyChanged(nameof(LastUsedStopBits));
                OnPropertyChanged(nameof(PortName));
            };
        }




        #region Computed properties
        private Func<SerialWrapper, int>? _indexer;
        public Func<SerialWrapper, int>? Indexer
        {
            get => _indexer;
            set => SetProperty(ref _indexer, value, () =>
            {
                OnPropertyChanged(nameof(Index));
            });
        }

        /// <summary>
        /// Determines the index of the Port in the list of serial settings containers available.
        /// </summary>
        public int Index => Indexer?.Invoke(this) ?? 1;

        /// <summary>
        /// String format of IP-address and port
        /// </summary>
        public string IPAddress => $"{SocketTools.IPAddress}:{_container.BroadcastPort}";

        /// <summary>
        /// Property that determines if SerialPort is opened
        /// </summary>
        private bool IsPortOpen => _serialPort?.IsOpen == true;

        /// <summary>
        /// Baud rate to use with SerialPort
        /// </summary>
        internal int? LastUsedBaudRate => _container.BaudRate;

        /// <summary>
        /// Data bits to use with SerialPort
        /// </summary>
        internal int? LastUsedDataBits => _container.Databits;

        /// <summary>
        /// 
        /// </summary>
        public string PortTooltip => ReadSwitch && ListeningFlag
                    ? $"Receiving readings from {PortName} and broadcasting over {IPAddress}"
                    : ReadSwitch
                        ? $"Receiving readings from {PortName}, not broadcasting currently"
                        : $"Currently not doing anything with {PortName}";

        /// <summary>
        /// Parity to use with SerialPort
        /// </summary>
        private Parity? LastUsedParity => _container?.Parity switch
        {
            NoParity => Parity.None,
            EvenParity => Parity.Even,
            OddParity => Parity.Odd,
            _ => null,
        };

        /// <summary>
        /// Takes string flow control value and returns the corresponding FlowControl enumeration value
        /// </summary>
        internal FlowControl? GetFlowControl() => _container.FlowControl switch
        {
            FlowControlCtsRts => FlowControl.Ctr_Rts,
            FlowControlDsrDtr => FlowControl.Dsr_Dtr,
            FlowControlXonXoff => FlowControl.Xon_Xoff,
            FlowControlNone => FlowControl.None,
            _ => null,
        };

        /// <summary>
        /// Takes string parity value and returns the corresponding Parity enumeration value
        /// </summary>
        internal Parity? GetParity() => _container.Parity switch
        {
            EvenParity => Parity.Even,
            OddParity => Parity.Odd,
            NoParity => Parity.None,
            _ => null,
        };

        /// <summary>
        /// Port ID to use for SerialPort connection
        /// </summary>
        public string PortName => _container.SerialPort;

        private int _broadcastPort = 0;
        /// <summary>
        /// Port for socket broadcast
        /// </summary>
        public int BroadcastPort
        {
            get => _broadcastPort;
            set => SetProperty(ref _broadcastPort, value, () =>
            {
                if (_broadcastPort > 65535)
                {
                    _broadcastPort = 65535;
                    OnPropertyChanged();
                }
                else if (BroadcastPort < 1)
                {
                    _broadcastPort = 1;
                    OnPropertyChanged();
                }

                _container.BroadcastPort = value;
            });
        }

        /// <summary>
        /// Stop bits to use for SerialPort connection
        /// </summary>
        internal StopBits? LastUsedStopBits => _container.StopBits switch
        {
            1 => StopBits.One,
            2 => StopBits.Two,
            _ => null,
        };

        /// <summary>
        /// This checks to see if the listening socket has been instantiated and is open to send values over
        /// </summary>
        internal bool SocketAvailable => _listener is not null && ListeningFlag;
        #endregion

        #region Read-write properties
        private Action<string, int>? _valueUpdatedCallback = (_, _) => { };
        /// <summary>
        /// Callback to be invoked every time the value from the SerialPort is updated
        /// </summary>
        internal Action<string, int>? ValueUpdatedCallback
        {
            get => _valueUpdatedCallback;
            set
            {
                if (_valueUpdatedCallback != value)
                {
                    _valueUpdatedCallback = value;
                }
            }
        }

        private string? _lastSerialReading;
        /// <summary>
        /// Last reading retrieved from the SerialPort
        /// </summary>
        public string LastSerialReading
        {
            get => _lastSerialReading ?? "";
            set
            {
                _lastSerialReading = value;
                OnPropertyChanged();

                _container.AddToReceivedValues(_lastSerialReading);

                ValueUpdatedCallback?.Invoke(_lastSerialReading, Index);
                OnPropertyChanged(nameof(SocketButtonEnabled));
                OnPropertyChanged(nameof(LastProcessedValue));
            }
        }

        /// <summary>
        /// Callback to be invoked if an expected exception is thrown
        /// </summary>
        internal Func<AvaloniaToolboxException, int?, Task> _threadExceptionCallback { get; set; } = (_, _) => Task.Run(() => { });
        #endregion

        #region Fields
        /// <summary>
        /// Socket that is being listened on
        /// </summary>
        private TcpListener? _listener;

        private bool _listeningFlag = false;
        /// <summary>
        /// Flag that indicates whether any values are being transmitted over the socket
        /// </summary>
        public bool ListeningFlag
        {
            get => _listeningFlag;
            set => SetProperty(ref _listeningFlag, value, () =>
            {
                Extensions.RunOnUIThread(() =>
                {
                    OnPropertyChanged(nameof(SocketButtonText));
                    OnPropertyChanged(nameof(BroadcastButtonIcon));
                    OnPropertyChanged(nameof(BroadcastButtonIcon));
                    OnPropertyChanged(nameof(Active));
                    OnPropertyChanged(nameof(IPAddress));
                    OnPropertyChanged(nameof(SocketControlsEnabled));
                    OnPropertyChanged(nameof(PortTooltip));
                }, DispatcherPriority.DataBind);
            });
        }

        private bool _readSwitch = false;
        /// <summary>
        /// Flag that indicates whether should be read from the SerialPort or not
        /// </summary>
        public bool ReadSwitch
        {
            get => _readSwitch;
            set => SetProperty(ref _readSwitch, value, () =>
            {
                Extensions.RunOnUIThread(() =>
                {
                    OnPropertyChanged(nameof(SerialButtonText));
                    OnPropertyChanged(nameof(BroadcastButtonIcon));
                    OnPropertyChanged(nameof(BroadcastButtonIcon));
                    OnPropertyChanged(nameof(Active));
                    OnPropertyChanged(nameof(SocketButtonEnabled));
                    OnPropertyChanged(nameof(PortTooltip));
                }, DispatcherPriority.DataBind);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        internal Action<int> _removeCallback = (_) => { };

        /// <summary>
        /// Text to display on serial button
        /// </summary>
        public string SerialButtonText => ReadSwitch ? "Stop" : "Start";

        /// <summary>
        /// Text to display on socket button
        /// </summary>
        public string SocketButtonText => ListeningFlag ? "Stop" : "Start";
#nullable enable
        /// <summary>
        /// SerialPort instance to be used throughout program
        /// </summary>
        private SerialPort? _serialPort = null;
#nullable restore

        internal readonly SerialSettingsContainer _container;
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        private Thread BeginAcceptSocketThread => new(new ThreadStart(BeginAcceptSocket))
        {
            IsBackground = true
        };

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="FarFulcrumException"></exception>
        private void BeginAcceptSocket()
        {
            ListeningFlag = true;
            do
            {
                LastProcessedValue = ProcessReading();

                string transmitValue = string.Empty;
                if (_lastProcessedValue == InvalidValue)
                {
                    transmitValue = InvalidValue;
                }

                string sendVal = _lastProcessedValue;
                //We check if value has a comma
                bool usesComma = sendVal.Contains(',');
                //If value has a comma, we replace all dots with nulls
                sendVal = usesComma ? sendVal.Replace(".", null)
                    : sendVal.Replace(",", null);
                //If more than one comma or dot is found, then we return "NAN"
                if (usesComma && sendVal.Count(i => i == ',') > 1)
                {
                    sendVal = InvalidValue;

                }
                else if (usesComma == false && sendVal.Count(i => i == '.') > 1)
                {
                    sendVal = InvalidValue;
                }
                //If value is not "NAN" and is parse-able as float, we then send value to socket
                if (sendVal != InvalidValue && float.TryParse(
                    sendVal,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out float result))
                {
                    transmitValue = sendVal;
                }
                else
                {
                    transmitValue = InvalidValue;
                }

                if (_listener is null || _listener.LocalEndpoint is null)
                {
                    throw new AvaloniaToolboxException("Could not get socket listener started.");
                }

                if (string.IsNullOrWhiteSpace(transmitValue) || _listener is null)
                {
                    return;
                }

                try
                {
                    if (_listener.Pending())
                    {
                        _ = _listener.BeginAcceptSocket(new AsyncCallback(DoAcceptSocketCallback), (_listener, transmitValue));
                    }
                }
                catch (SocketException ex)
                {
                    ListeningFlag = false;
                    throw new AvaloniaToolboxException("Exception occurred during socket data send.", ex);
                }
                Thread.Sleep(200);
            } while (ListeningFlag);
        }

        /// <summary>
        /// This starts the process of sending processed values over the socket
        /// </summary>
        public void BroadcastSerialValuesCommand()
        {
            if (ListeningFlag == false)
            {
                _ = Task.Run(() =>
                {
                    StartServer();
                    BeginAcceptSocketThread.Start();
                });
            }
            else
            {
                StopServer();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        private void DoAcceptSocketCallback(IAsyncResult asyncResult)
        {
            if (asyncResult.AsyncState is null || SocketAvailable == false)
            {
                return;
            }

            (TcpListener? listener, string? stringValue) = ((TcpListener?, string?))asyncResult.AsyncState;

            if (listener is null || stringValue is null)
            {
                return;
            }

            try
            {
                using (Socket handler = listener.EndAcceptSocket(asyncResult))
                {
                    byte[] msg = Encoding.ASCII.GetBytes(stringValue);

                    if (handler is not null)
                    {
                        if (handler.Connected)
                        {
                            _ = handler.Send(msg);
                        }
                    }

                    handler?.Shutdown(SocketShutdown.Receive);
                    handler?.Close();
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (SocketException e)
            {
                CustomDebug.WriteLine($"An error occurred when attempting to access the socket.\n{e.Message}");
            }
        }

        /// <summary>
        /// This gets the port id and last saved settings in CustomSettings to instantiate a SerialPort and start reading from it
        /// </summary>
        private void GetPortAndStartListening()
        {
            if (string.IsNullOrWhiteSpace(PortName))
            {
                throw new AvaloniaToolboxException("Selected port is blank.");
            }
            else if (LastUsedBaudRate is null)
            {
                throw new AvaloniaToolboxException("An invalid baud rate has been specified. Reselect baud rate and try again.");
            }
            else if (LastUsedDataBits is null)
            {
                throw new AvaloniaToolboxException("An invalid data bits value has been specified. Reselect data bits and try again.");
            }
            else if (LastUsedStopBits is null or not StopBits)
            {
                throw new AvaloniaToolboxException("Invalid value for stop bits encountered.");
            }
            else if (LastUsedParity is null or not Parity)
            {
                throw new AvaloniaToolboxException("Invalid value for parity encountered.");
            }
            else if (string.IsNullOrWhiteSpace(_container.FlowControl))
            {
                throw new AvaloniaToolboxException("An invalid flow control value has been specified. Reselect flow control and try again.");
            }
            else if (_container.SerialTimeoutMs is null or < 500)
            {
                throw new AvaloniaToolboxException("An invalid timeout value has been specified. Reselect timeout and try again.");
            }
            else if (LastUsedBaudRate != null && LastUsedDataBits != null && _container.SerialTimeoutMs != null && LastUsedParity != null && LastUsedParity is Parity castedParity && LastUsedStopBits != null && LastUsedStopBits is StopBits castedStopBits)
            {
                _serialPort = new SerialPort(PortName, (int)LastUsedBaudRate, castedParity, (int)LastUsedDataBits, castedStopBits)
                {
                    ReadTimeout = _container.SerialTimeoutMs ?? s_defaultTimeout
                };

                ReadSwitch = true;
            }

            if (_serialPort != null)
            {

                if (_serialPort.IsOpen)
                {
                    throw new AvaloniaToolboxException("Serial port is already open, re-select port and try again.");
                }
                try
                {
                    _serialPort.Open();
                    if (IsPortOpen == false)
                    {
                        throw new AvaloniaToolboxException($"Could not connect to serial port: {_serialPort.PortName}");
                    }

                    StartReadingSerialPortWithTimeout();
                }
                catch (UnauthorizedAccessException e)
                {
                    throw new AvaloniaToolboxException("Another process on the system already has the specified COM port open. Re-select port and try again.", e);
                }
            }
        }

        /// <summary>
        /// Command that initializes listening to the serial port
        /// </summary>
        public async void ListenToSerialCommand()
        {
            if (ReadSwitch)
            {
                await Extensions.InvokeOnUiThread(MessageBox.ShowProgressDialog(
                    null,
                    "Closing serial port",
                    "Closing serial port and any open socket connections.",
                    () =>
                    {
                        ValueUpdatedCallback = (_, _) => { };
                        StopListeningForcefully();
                        LastSerialReading = string.Empty;
                        ListeningFlag = false;
                        StopServer();
                    }
                ));
            }
            else
            {
                try
                {
                    GetPortAndStartListening();
                }
                catch (AvaloniaToolboxException exception)
                {
                    await _threadExceptionCallback(exception, Index);
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemovePortCommand()
        {
            _removeCallback.Invoke(Index);
        }

        /// <summary>
        /// Command that initiates the QR code callback to handle creating and displaying the QR code.
        /// </summary>
        public async void ShowQRCodeCommand()
        {
            if (ListeningFlag && ReadSwitch)
            {
                QRCodeGenerator qrGenerator = new();
                QRCodeData data = qrGenerator.CreateQrCode($"{SocketTools.IPAddress}:{_container.BroadcastPort}", QRCodeGenerator.ECCLevel.Q);
                await MessageBox.Show(null, ViewFunctions.GetImageBitmap(new BitmapByteQRCode(data).GetGraphic(20)));
            }
            else
            {
                _ = await MessageBox.Show(null, "Error", "Can not show QR code when socket broadcast is inactive.");
            }
        }

        /// <summary>
        /// Thread that starts the process of reading from the serial port.
        /// </summary>
        private Thread StartReadingFromPortThread => new(new ThreadStart(() =>
        {
            _ = Task.Run(async () =>
            {
                if (ReadSwitch)
                {
                    
                }

                while (ReadSwitch)
                {
                    try
                    {
                        if (_serialPort == null)
                        {
                            await _threadExceptionCallback.Invoke(new AvaloniaToolboxException("Serial port listener was never instantiated."), Index);
                            CustomDebug.WriteLine($"Serial port listener was never instantiated. {Index}");
                        }
                        else
                        {
                            string message = string.Empty;

                            if (_serialPort is not null)
                            {
                                message = _serialPort.ReadLine();
                            }

                            if (_serialPort is not null)
                            {
                                LastSerialReading = message;
                            }

                            if (_serialPort is not null)
                            {
                                _serialPort.DiscardInBuffer();
                            }
                        }
                    }
                    catch (TimeoutException timeoutException)
                    {
                        CustomDebug.WriteLine($"Serial port did not return a value in time {Index}");
                        await _threadExceptionCallback.Invoke(new AvaloniaToolboxException("Serial port did not return a value in time", timeoutException), Index);
                    }
                }
            });
        }))
        {
            IsBackground = true
        };

        /// <summary>
        /// This method starts reading from created serial port and sets the static last read value on every read
        /// </summary>
        private void StartReadingSerialPortWithTimeout()
        {
            StartReadingFromPortThread.Start();
        }

        /// <summary>
        /// This stops listening on SerialPort in a forceful manner, by setting the SerialPort to null and toggling read flag
        /// </summary>
        internal void StopListeningForcefully()
        {
            _serialPort?.Close();
            _serialPort = null;

            ReadSwitch = false;
        }

        /// <summary>
        /// This stops listening on SerialPort by merely closing the SerialPort and toggling read flag
        /// </summary>
        internal void StopListeningOnPort()
        {
            if (_serialPort?.IsOpen == true)
            {
                _serialPort.Close();
            }

            ReadSwitch = false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateIndex() => OnPropertyChanged(nameof(Index));

        /// <summary>
        /// Processes the serial reading according to user-configured specifications
        /// </summary>
        /// <returns>Returns altered string reading or "NAN" if any of the checks fail.</returns>
        private string ProcessReading()
        {
            if (string.IsNullOrWhiteSpace(LastSerialReading))
            {
                return string.Empty;
            }

            if (ReadSwitch == false)
            {
                return string.Empty;
            }
            if (_container.StabilityIndicatorActive && _container.StabilityIndicatorSnippet is not null)
            {
                if (LastSerialReading.Contains(_container.StabilityIndicatorSnippet) == false)
                {
                    return InvalidValue;
                }

                string refSnippet = LastSerialReading.Substring(_container.StabilityIndicatorStartingPosition - 1, _container.StabilityIndicatorSnippet.Length);
                if (refSnippet != _container.StabilityIndicatorSnippet)
                {
                    return InvalidValue;
                }
            }
            else if (_container.SequenceOfIdenticalReadingsActive)
            {
                if (_container.NumberOfIdenticalReadings > _container.ReceivedValues.Count)
                {
                    return InvalidValue;
                }
                else if (string.IsNullOrWhiteSpace(LastSerialReading))
                {
                    return InvalidValue;
                }
                else
                {
                    string identicalReadingToLookFor = LastSerialReading;
                    for (int i = 0; i < _container.NumberOfIdenticalReadings; i++)
                    {
                        if (_container.ReceivedValues[i] == identicalReadingToLookFor)
                        {
                            continue;
                        }
                        else
                        {
                            return InvalidValue;
                        }
                    }
                }
            }
            if (_container.ScaleStringWeightStartingPosition >= 1 && _container.ScaleStringWeightStartingPosition > _container.ScaleStringWeightEndingPosition)
            {
                return InvalidValue;
            }
            else if (_container.ScaleStringWeightStartingPosition > LastSerialReading.Length)
            {
                return InvalidValue;
            }
            else if (_container.ScaleStringMustConformToLength && _container.ScaleStringRequiredLength > 0 && LastSerialReading.Length != _container.ScaleStringRequiredLength)
            {
                return InvalidValue;
            }
            else
            {
                if (_container.ScaleStringWeightEndingPosition is not null && _container.ScaleStringWeightStartingPosition is not null)
                {
                    string userSpecifiedString = LastSerialReading.Length >= _container.ScaleStringWeightEndingPosition
                        ? LastSerialReading[((int)_container.ScaleStringWeightStartingPosition - 1)..(int)_container.ScaleStringWeightEndingPosition]
                        : LastSerialReading[((int)_container.ScaleStringWeightStartingPosition - 1)..LastSerialReading.Length];

                    return userSpecifiedString.Any(char.IsLetter)
                        ? new string(userSpecifiedString.Where(i => char.IsNumber(i) || i == '.' || i == ',').ToArray())
                        : userSpecifiedString;
                }

                return InvalidValue;
            }
        }

        /// <summary>
        /// This instantiates the listening socket, binds it to an appropriate endpoint, sets backlog and listening flag
        /// </summary>
        private void StartServer()
        {
            try
            {
                _listener = new(SocketTools.EndPointIPAddress, _container.BroadcastPort);
                _listener.Start();
            }
            catch (SocketException exception)
            {
                ListeningFlag = false;
                throw new AvaloniaToolboxException("TCP listener was never instantiated", exception);
            }
        }

        /// <summary>
        /// This closes the socket and stops any connections from being made to the listening socket
        /// </summary>
        internal void StopServer()
        {
            if (_listener is not null)
            {
                ListeningFlag = false;
                _listener?.Stop();
                _listener = null;
            }
        }


        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of this wrapper, the associated Serial port and socket.
        /// </summary>
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {

            }

            StopServer();
            StopListeningForcefully();
        }
        #endregion
    }
}
