using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login_Integration_Application
{
    public class Application_Properties
    {
        /// <summary>
        /// Keeps track of USBEvent
        /// </summary>
        public USBEvent USBEventType { get; set; }

        /// <summary>
        /// Keeps track of number of connected devices
        /// </summary>
        public int NumberOfConnectedDevice { get; set; }

        // Device vid and pid
        public string DeviceVID;
        public string DevicePID;

        public Hashtable PortStatus;
        public Hashtable CurrentPortStatus;

        public Application_Properties()
        {
            PortStatus = new Hashtable();
            CurrentPortStatus = new Hashtable();

            // Initializes Usb Ports statuses
            ApplicationMethods.initializePortStatus(PortStatus);
            ApplicationMethods.initializePortStatus(CurrentPortStatus);
        }
    }
}
