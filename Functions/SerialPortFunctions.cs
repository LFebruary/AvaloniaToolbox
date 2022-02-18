using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using AvaloniaToolbox.Models;

namespace AvaloniaToolbox.Functions
{
    public static partial class SerialPortFunctions
    {
        /// <summary>
        /// Takes a list of serial wrappers containers and returns only the serial port names
        /// </summary>
        /// <param name="value"></param>
        private static IEnumerable<string> SerialPortIDList(this IEnumerable<SerialWrapper> value) => value.Select(i => i.PortName);

        /// <summary>
        /// Takes an observable collection of serial wrappers and returns only the serial port names
        /// </summary>
        /// <param name="value"></param>
        private static IEnumerable<string> SerialPortIDList(this ObservableCollection<SerialWrapper> value) => value.ToList().SerialPortIDList();

        /// <summary>
        /// Takes a list of serial settings containers and returns only the serial port names
        /// </summary>
        /// <param name="value"></param>
        private static IEnumerable<string> SerialPortIDList(this IEnumerable<SerialSettingsContainer> value) => value.Select(i => i.SerialPort);

        /// <summary>
        /// These are all the serial ports available to the machine
        /// </summary>
        internal static IEnumerable<string> AvailableSerialPorts => SerialPort.GetPortNames();
    }
}
