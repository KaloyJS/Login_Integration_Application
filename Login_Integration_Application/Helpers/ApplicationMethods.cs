using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System;
using System.Windows;
using System.Text.RegularExpressions;
using MessageBox = System.Windows.MessageBox;
using Newtonsoft.Json;
using System.Reflection;

namespace Login_Integration_Application
{
    class ApplicationMethods
    {
        #region portstatus functions

        /// <summary>
        /// Initializes port status hash table
        /// </summary>
        public static void initializePortStatus(Hashtable hashtable)
        {
            hashtable.Add("1", "Empty");
            hashtable.Add("2", "Empty");
            hashtable.Add("3", "Empty");
        }



        /// <summary>
        /// Reset port status
        /// </summary>
        public static void resetPortStatus(Hashtable hashtable)
        {
            hashtable["1"] = "Empty";
            hashtable["2"] = "Empty";
            hashtable["3"] = "Empty";
        }

        #endregion

        #region Helper Functions
        /// <summary>
        /// Sets the Headers Dynamically
        /// </summary>
        /// <param name="obj">Device obj</param>
        /// <param name="path">Path of Icon</param>
        /// <param name="msg">Header messge</param>
        public static void setHeaders(Device obj, string path, string msg)
        {
            obj.Header = msg;
            obj.StatusIcon = path;
            obj.Status = msg;
        }

        public static void showConnectedDevice(List<string> connectedDevicesUDID)
        {
            string output = "";
            for (int i = 0; i < connectedDevicesUDID.Count; i++)
            {
                if (i != 0)
                    output += "\n";

                output += connectedDevicesUDID[i];



            }
            MessageBox.Show(output);
        }

        

        /// <summary>
        /// Prompts an error message and updates Port Headers of designated port
        /// </summary>
        /// <param name="device"></param>
        /// <param name="msg"></param>
        public static void ShowError(Device device, string msg)
        {
            //MessageBox.Show(msg);
            device.Status = msg;
            device.Header = "Disconnect Device";
            device.StatusIcon = PortStatusIconPath.WarningIcon;
        }

        /// <summary>
        /// Returns true if only Single UDID is recorded , false if two or more 
        /// </summary>
        /// <param name="UDID"></param>
        /// <returns></returns>
        public static Boolean checkUDID(string UDID)
        {
            // Splits UDID on New lines
            string[] lines = Regex.Split(UDID, "\r\n");

            if (lines.Length == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Resets Device class properties
        /// </summary>
        /// <param name="device">Which device class to reset</param>
        public static void clearDevice(Device device)
        {
            device.Reset();
        }


        /// <summary>
        /// Finds all controls in the WPF WIndow by type
        /// Usage: foreach (TextBox t in HelperFunctions.FindLogicalChildren<TextBox>(this)) { }
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (object rawChild in LogicalTreeHelper.GetChildren(depObj))
                {
                    if (rawChild is DependencyObject)
                    {
                        DependencyObject child = (DependencyObject)rawChild;
                        if (child is T)
                        {
                            yield return (T)child;
                        }

                        foreach (T childOfChild in FindLogicalChildren<T>(child))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }

        #endregion

        #region UDID functions

        /// <summary>
        /// Displays connected Device UDID in a string
        /// </summary>
        /// <param name="connectedDevice"></param>
        /// <returns></returns>
        public static string showString(List<string> connectedDevice)
        {
            string output = "";
            for (int i = 0; i < connectedDevice.Count; i++)
            {
                if (i != 0)
                    output += "\n";

                output += connectedDevice[i];



            }

            return output;
        }


        /// <summary>
        /// Removes the connected UDID from the Connected UDIDS array
        /// </summary>
        /// <param name="UDID"></param>
        /// <param name="NumberOfConnectedDevice"></param>
        public static void removeUDID(string UDID, int NumberOfConnectedDevice, List<string> connectedDevicesUDID, string ConnectedUDIDs)
        {
            if (NumberOfConnectedDevice == 0)
                connectedDevicesUDID.Clear();
            else
                connectedDevicesUDID.Remove(UDID);

            ConnectedUDIDs = ApplicationMethods.showString(connectedDevicesUDID);

        }



        #endregion


        #region Save Data


        /// <summary>
        /// Method that checks if Device Properties is ready to be saved and saves to a PHP endpoint by post 
        /// </summary>
        /// <param name="device"></param>
        public static void SaveData(Device device)
        {
            string status;
            if (string.IsNullOrEmpty(device.CodePro))
            {
                ShowError(device, "Reconnect Device Again");
            }
            else if (device.CodePro == "N/A")
            {
                ShowError(device, "CodePro not mapped, Please disconnect device and contact Data Department ASAP");
            }
            else
            {
                // Check if Properties are set and not empty
                if (device.isReadyToBeSaved())
                {
                    // Pushing data into table and recieving response
                    status = Apple_Methods.PushData(device);

                    //MessageBox.Show(status);

                    // Showing prompt for Error Responses
                    if (status != "CSV Uploaded")
                    {
                        ShowError(device, status);
                    }                    

                    // Showing Response as status
                    device.Status = status;
                }
                else
                {                    
                    ShowError(device, "Not all properties are detected, please reconnect device");
                }    
            }
           
               
        }

        #endregion

        #region On Connect process

        /// <summary>
        /// Processing Device on port connect
        /// </summary>
        public static void OnConnectPortProcess(string port, Device device, int NumberOfConnectedDevice, PortConnectionViewModel portConnectionViewModel)
        { 
            // Get Device UDID
            device.UDID = Apple_Methods.GetUdid(NumberOfConnectedDevice, portConnectionViewModel.connectedDevicesUDID);
            //Check if UDID is valid
            if (ApplicationMethods.checkUDID(device.UDID) && !string.IsNullOrEmpty(device.UDID))
            {
                // Adding Device UDID to array
                portConnectionViewModel.connectedDevicesUDID.Add(device.UDID);
                // Converts ConnectedDevicesUDID array into a string to display
                portConnectionViewModel.ConnectedUDIDs = ApplicationMethods.showString(portConnectionViewModel.connectedDevicesUDID);

                // Pair device to libimobiledevice
                device.PairStatus = Apple_Methods.PairDevice(device.UDID);
                //portConnectionViewModel.Device_1.Status = portConnectionViewModel.Device_1.PairStatus;
                device.IMEI = Apple_Methods.GetIMEI(device.UDID);
                if (string.IsNullOrWhiteSpace(device.IMEI))
                {
                    MessageBox.Show("Could not get device properties, disconnect device and try again. If Error persist contact data department!");
                }
                else
                {
                    device.Header = "Device Connected";
                    device.StatusIcon = PortStatusIconPath.ConnectedIcon;
                    device.OEM = device.TranslateManufacturer(device.Manufacturer);
                    device.Serial_Number = Apple_Methods.GetSerialNumber(device.UDID);
                    device.ModelNumber = Apple_Methods.GetModelNumber(device.UDID);
                    device.Model = Apple_Methods.GetModelName(device.ModelNumber);
                    device.Software_Version = Apple_Methods.GetSoftwareVersion(device.UDID);
                    device.Color = Apple_Methods.GetColor(device.ModelNumber);
                    device.Capacity = Apple_Methods.GetCapacity(device.ModelNumber);
                    device.FMIP = Apple_Methods.IsFMIPLocked(device.IMEI);
                    device.MDM_Lock = Apple_Methods.IsMDM(device.UDID);
                    device.CodePro = Apple_Methods.GetCodePro(device.ModelNumber);
                    device.Jobnumber = Apple_Methods.GetJobnumber(device.IMEI);
                }
            }
            else
            {
                // When two or more devices are connected on startup Error
                MessageBox.Show("Disconnect devices and connect again, if error persist contact data team");
                ClearFields(port, NumberOfConnectedDevice, portConnectionViewModel);
            }
            

        }

        /// <summary>
        /// On Connect of Device
        /// </summary>
        public static void OnConnect(string port, PortConnectionViewModel portConnectionViewModel, int NumberOfConnectedDevice)
        {
            // Depends on which port is connected
            switch (port)
            {
                case "1":
                    OnConnectPortProcess(port, portConnectionViewModel.Device_1, NumberOfConnectedDevice, portConnectionViewModel);                    
                    break;

                case "2":
                    OnConnectPortProcess(port, portConnectionViewModel.Device_2, NumberOfConnectedDevice, portConnectionViewModel);                    
                    break;

                case "3":
                    OnConnectPortProcess(port, portConnectionViewModel.Device_3, NumberOfConnectedDevice, portConnectionViewModel);                    
                    break;
            }
        }

        #endregion


        #region clear fields function

        /// <summary>
        /// Clear fields by port
        /// </summary>
        public static void ClearFields(string port, int NumberOfConnectedDevice, PortConnectionViewModel portConnectionViewModel)
        {
            switch (port)
            {
                case "1":
                    // Removes UDID from the connectedDevicesUDID array
                    removeUDID(portConnectionViewModel.Device_1.UDID, NumberOfConnectedDevice, portConnectionViewModel.connectedDevicesUDID, portConnectionViewModel.ConnectedUDIDs);
                    // Resets the Device Object
                    portConnectionViewModel.Device_1.Reset();
                    // Disables the print button
                    portConnectionViewModel.Port1_Button = false;
                    // Resets the button content
                    portConnectionViewModel.Port1_ButtonContent = "Save Device Info";
                    // Disables Invoice Date picker
                    portConnectionViewModel.Port1_InvoiceDate = false;
                    // Resets Datepicker
                    
                    break;

                case "2":
                    // Removes UDID from the connectedDevicesUDID array
                    removeUDID(portConnectionViewModel.Device_2.UDID, NumberOfConnectedDevice, portConnectionViewModel.connectedDevicesUDID, portConnectionViewModel.ConnectedUDIDs);
                    // Resets the Device Object
                    portConnectionViewModel.Device_2.Reset();
                    // Disables the print button
                    portConnectionViewModel.Port2_Button = false;
                    // Resets the button content
                    portConnectionViewModel.Port2_ButtonContent = "Save Device Info";
                    // Disables Invoice Date picker
                    portConnectionViewModel.Port2_InvoiceDate = false;
                    // Resets Datepicker
                    break;

                case "3":
                    // Removes UDID from the connectedDevicesUDID array
                    removeUDID(portConnectionViewModel.Device_3.UDID, NumberOfConnectedDevice, portConnectionViewModel.connectedDevicesUDID, portConnectionViewModel.ConnectedUDIDs);
                    // Resets the Device Object
                    portConnectionViewModel.Device_3.Reset();
                    // Disables the print button
                    portConnectionViewModel.Port3_Button = false;
                    // Resets the button content
                    portConnectionViewModel.Port3_ButtonContent = "Save Device Info";
                    // Disables Invoice Date picker
                    portConnectionViewModel.Port3_InvoiceDate = false;
                    // Resets Datepicker
                    break;
            }
        }

        #endregion


        #region USB Port Functions



        /// <summary>
        /// Gets the disconnected/Connected Port of USB devices by comparing current port statuses and last port statuses
        /// </summary>
        /// <param name="CurrentPortStatus">Snapshot of Current Port status from GetCurrentPortStatus method</param>
        /// <param name="PortStatus">PortStatus</param>
        /// <returns></returns>

        public static string GetPort( Application_Properties app)
        {
            string port = "";

            foreach (DictionaryEntry de in app.PortStatus)
            {
                if (app.PortStatus[de.Key] != app.CurrentPortStatus[de.Key])
                {
                    port = de.Key.ToString();
                    break;
                }
            }

            app.PortStatus[port] = app.CurrentPortStatus[port];

            return port;
        }



        #endregion

        #region dynamically set object property

        /// <summary>
        /// Method that determines which device to update by passing the elementName where the Method is invoked on
        /// </summary>
        /// <param name="propertyName">Device property name</param>
        /// <param name="portConnectionViewModel">data context object</param>
        /// <param name="elementName">element name of where the method is invoked from</param>
        /// <param name="value">value</param>
        public static void SetDeviceProperty(string propertyName, PortConnectionViewModel portConnectionViewModel, string elementName, string value)
        {
            PropertyInfo propertyInfo;
            Device obj;
            if (elementName.Contains("1"))
            {
                propertyInfo = portConnectionViewModel.Device_1.GetType().GetProperty(propertyName);
                obj = portConnectionViewModel.Device_1;
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(obj, value, null);
                }
            }
            else if (elementName.Contains("2"))
            {
                propertyInfo = portConnectionViewModel.Device_2.GetType().GetProperty(propertyName);
                obj = portConnectionViewModel.Device_2;
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(obj, value, null);
                }
            }
            else if (elementName.Contains("3"))
            {
                propertyInfo = portConnectionViewModel.Device_3.GetType().GetProperty(propertyName);
                obj = portConnectionViewModel.Device_3;
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(obj, value, null);
                }
            }            
            
        }

        private void SetObjectProperty(string propertyName, string value, object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            // make sure object has the property we are after
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value, null);
            }
        }

        #endregion
    }
}
