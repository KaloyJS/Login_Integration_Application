using System;
using System.Collections.Generic;
using USBClassLibrary;

namespace Login_Integration_Application
{
    class USBClassLibrary_Methods
    {
        /// <summary>
        /// Returns the Port location of the device connected
        /// </summary>
        public static void GetCurrentPortStatus(Application_Properties App, PortConnectionViewModel portConnectionViewModel)
        {
            //declaring an instance of USBClass
            USBClassLibrary.USBClass USBPort = new USBClass();


            //an instance List<T> of DeviceProperties if you want to read the properties of your devices
            List<USBClassLibrary.USBClass.DeviceProperties> ListOfUSBDeviceProperties;

            ListOfUSBDeviceProperties = new List<USBClassLibrary.USBClass.DeviceProperties>();

            Nullable<UInt32> MI = null;
            bool bGetSerialPort = false;


            if (USBClass.GetUSBDevice(uint.Parse(App.DeviceVID, System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse(App.DevicePID, System.Globalization.NumberStyles.AllowHexSpecifier), ref ListOfUSBDeviceProperties, bGetSerialPort, MI))
            {

                // Creates an array of Connected Devices 
                List<string> connectedDevices = new List<string>();
                for (int i = 0; i < ListOfUSBDeviceProperties.Count; i++)
                {
                    string Manufacturer = ListOfUSBDeviceProperties[i].DeviceManufacturer;
                    string currentPort = ListOfUSBDeviceProperties[i].DeviceLocation.Substring(ListOfUSBDeviceProperties[0].DeviceLocation.IndexOf('#') + 4, 1);


                    //MessageBox.Show(currentPort);
                    if (int.Parse(currentPort) <= 3)
                    {
                        connectedDevices.Add(currentPort.ToString());
                        if (currentPort.ToString() == "1")
                        {
                            portConnectionViewModel.Device_1.Manufacturer = Manufacturer;
                        }
                        else if (currentPort.ToString() == "2")
                        {
                            portConnectionViewModel.Device_2.Manufacturer = Manufacturer;
                        }
                        else if (currentPort.ToString() == "3")
                        {
                            portConnectionViewModel.Device_3.Manufacturer = Manufacturer;
                        }

                    }


                }
                //string toDisplay = string.Join(Environment.NewLine, connectedDevices);
                //MessageBox.Show(toDisplay);
                ApplicationMethods.resetPortStatus(App.CurrentPortStatus);
                foreach (string device in connectedDevices)
                {
                    App.CurrentPortStatus[device] = "Connected";
                }


            }
        }
    }
}
