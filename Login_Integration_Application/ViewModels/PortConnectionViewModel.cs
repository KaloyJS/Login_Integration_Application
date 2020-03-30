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
        /// Determines the Button is enabled/disabled
        /// </summary>
        
        public bool Port1_Button { get; set; }

        public bool Port2_Button { get; set; }

        public bool Port3_Button { get; set; }

        /// <summary>
        /// Determines the content of the port buttons
        /// </summary>
        public string Port1_ButtonContent { get; set; }

        public string Port2_ButtonContent { get; set; }

        public string Port3_ButtonContent { get; set; }

        /// <summary>
        /// Status of Date invoice date picker
        /// </summary>
        public bool Port1_InvoiceDate { get; set; }

        public bool Port2_InvoiceDate { get; set; }

        public bool Port3_InvoiceDate { get; set; }


        public PortConnectionViewModel()
        {
            // instantiate 3 devices (for ports 1 - 2), add additional of adding more ports
            Device_1 = new Device(1);
            Device_2 = new Device(2);
            Device_3 = new Device(3);
            // Instantiate a new list of connected Devices UDID
            connectedDevicesUDID = new List<string>();

            // disables invoice date datepicker on start
            Port1_InvoiceDate = false;
            Port2_InvoiceDate = false;
            Port3_InvoiceDate = false;

            // Instantiate Print Buttons

            Port1_Button = false;
            Port2_Button = false;
            Port3_Button = false;

            Port1_ButtonContent = "Save Device Info";
            Port2_ButtonContent = "Save Device Info";
            Port3_ButtonContent = "Save Device Info";


        }
    }
}
