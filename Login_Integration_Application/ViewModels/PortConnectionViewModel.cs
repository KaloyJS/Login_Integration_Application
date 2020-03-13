using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Login_Integration_Application
{
    public class PortConnectionViewModel : BaseViewModel
    {     
        /// <summary>
        /// Objects of the 3 ports are declared
        /// </summary>

        public Device Device_1 { get; set; }

        public Device Device_2 { get; set; }

        public Device Device_3 { get; set; }

        /// <summary>
        /// List of Connected Devices UDID
        /// </summary>
        public List<string> connectedDevicesUDID { get; set; }

        public string ConnectedUDIDs { get; set; }

        /// <summary>
        /// Print Button Status
        /// </summary>
        
        public bool Port1_PrintButton { get; set; }

        public bool Port2_PrintButton { get; set; }

        public bool Port3_PrintButton { get; set; }


        public PortConnectionViewModel()
        {
            // instantiate 3 devices (for ports 1 - 2), add additional of adding more ports
            Device_1 = new Device(1);
            Device_2 = new Device(2);
            Device_3 = new Device(3);
            // Instantiate a new list of connected Devices UDID
            connectedDevicesUDID = new List<string>();

            // Instantiate Print Buttons

            Port1_PrintButton = false;
            Port2_PrintButton = false;
            Port3_PrintButton = false;




        }
    }
}
