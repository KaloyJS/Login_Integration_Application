using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Login_Integration_Application
{

    public class Device : BaseViewModel
    {
        #region Public members
        /// <summary>
        /// Class for device properties
        /// </summary>
        ///        

        public string Manufacturer { set; get; }


        // Apple Devices Properties
        public string UDID { set; get; }

        public string ModelNumber { set; get; }

        public string FMIP { set; get; }

        public string MDM_Lock { set; get; }

        public string PairStatus { set; get; }

        // Universal Properties
        public string Header { set; get; }

        public int Port { set; get; }

        public string OEM { set; get; }

        public string Serial_Number { set; get; }

        public string IMEI { set; get; }

        public string Model { set; get; }

        public string Software_Version { set; get; }

        public string Jobnumber { set; get; }

        public string CodePro { set; get; }

        public string Color { set; get; }

        public string Capacity { set; get; }

        public string WorkStation { set; get; }

        public string UserName { set; get; }

        public string Status { set; get; }

        /// <summary>
        ///  Where Status of each port is housed
        /// </summary>
        public string StatusIcon { get; set; }

        public string InvoiceDate { get; set; }

        public string Warranty { get; set; }

        public string WarrantyCode { get; set; }

        public Device(int Port) 
        {
            this.Header = "Connect Device";
            this.Port = Port;
            this.WorkStation = System.Environment.MachineName;
            this.UserName = Environment.UserName;
            this.StatusIcon = PortStatusIconPath.DisconnectedIcon;

        }

        /// <summary>
        /// Resets the class properties
        /// </summary>
        public void Reset() {
            this.Header = "Connect Device";
            this.StatusIcon = PortStatusIconPath.DisconnectedIcon;
            // Reset everything except Port, WorkStation and UserName
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; ++i)
            {
                if (properties[i].Name != "Port" && properties[i].Name != "WorkStation" && properties[i].Name != "UserName" && properties[i].Name != "Header" && properties[i].Name != "StatusIcon")
                {
                    properties[i].SetValue(this, null);
                }
                
            }
                
        }

        public string TranslateManufacturer(string Manufacturer)
        {
            string oem = "N/A";

            if (Manufacturer == "Apple, Inc.")
                oem = "Apple";

            return oem;
        }

        /// <summary>
        /// Checks if properties derived from ideviceinfo is set 
        /// </summary>
        /// <returns></returns>
        public Boolean isReadyToBeSaved()
        {
            if (OEM != "N/A" && !string.IsNullOrEmpty(IMEI) && !string.IsNullOrEmpty(Serial_Number) && !string.IsNullOrEmpty(ModelNumber) && !string.IsNullOrEmpty(Software_Version))
                return true;
            else 
                return false;
        }




        #endregion
    }
}
