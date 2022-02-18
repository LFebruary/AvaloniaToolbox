using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace AvaloniaToolbox.Functions
{
    internal static partial class SocketTools
    {
        #region Computed properties
        /// <summary>
        /// IPAddress on which the socket will listen <br/>
        /// This method first tries to get an IPAddress for the Ethernet connection, if not then a wireless LAN connection
        /// </summary>
        internal static IPAddress EndPointIPAddress => System.Net.IPAddress.Parse(GetLocalIPV4Addresses(NetworkInterfaceType.Ethernet).FirstOrDefault()
            ?? GetLocalIPV4Addresses(NetworkInterfaceType.Wireless80211).FirstOrDefault()
            ?? throw new AvaloniaToolboxException("Could not determine local IP-address to broadcast serial readings on."));

        /// <summary>
        /// This merely converts the EndPoint IP address to a string for display on main window
        /// </summary>
        internal static string IPAddress => EndPointIPAddress.ToString();
        #endregion

        #region Methods

        /// <summary>
        /// Returns a list of IPV4 addresses associated with device program is running on
        /// </summary>
        /// <param name="interfaceType">Specific interface to find addresses for</param>
        /// <returns>List of string IP-addresses for specified interface</returns>
        private static List<string> GetLocalIPV4Addresses(NetworkInterfaceType interfaceType)
        {
            List<string> ipAddressList = new();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.Description.Contains("Hyper", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (item.Name.Contains("Virtual", System.StringComparison.InvariantCultureIgnoreCase) == false
                    && item.NetworkInterfaceType == interfaceType && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddressList.Add(ip.Address.ToString());
                        }
                    }
                }
            }

            return ipAddressList;
        }
        #endregion
    }
}
