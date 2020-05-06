using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LibUsbDotNet.DeviceNotify;


namespace Login_Integration_Application
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Application By Carlo Nayve
    /// SBE Application
    /// Description: Scans USB ports from Hub, gets Device Properties, saves and prints labels
    /// </summary>


    public partial class MainWindow : Window
    {
        #region Declarations

        private Application_Properties App;
        public PortConnectionViewModel portConnectionViewModel;
        //LibUsbDotNet Instance
        public static IDeviceNotifier UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();

        #endregion

        #region Main
        public MainWindow()
        {
            InitializeComponent();
            //Create Instance of Application Properties
            App = new Application_Properties();
            // Instance of view Model
            portConnectionViewModel = new PortConnectionViewModel();
            // Connect view model to datacontext for binding
            DataContext = portConnectionViewModel;
            UsbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent;

        }

        #endregion

        #region On connect/disconnect event handler

        /// <summary>
        /// Event handler for any connect and disconnect of USB Devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceNotifyEvent(object sender, DeviceNotifyEventArgs e)
        {
            // On USB connect
            if (e.EventType.ToString() == "DeviceArrival")
            {

                App.USBEventType = USBEvent.Connected;
                //Error Check
                if (e.Device != null)
                {
                    // sets Vid and Pid of connected device                    
                    App.DeviceVID = e.Device.IdVendor.ToString("X4");
                    App.DevicePID = e.Device.IdProduct.ToString("X4");
                    USBClassLibrary_Methods.GetCurrentPortStatus(App, portConnectionViewModel);
                    string ConnectedPort = ApplicationMethods.GetPort(App);
                    if (!string.IsNullOrEmpty(ConnectedPort))
                    {
                        App.NumberOfConnectedDevice++;
                        // Get Device Information and Assign to each port
                        ApplicationMethods.OnConnect(ConnectedPort, portConnectionViewModel, App.NumberOfConnectedDevice);
                    }


                }

            }
            // On disconnect
            else
            {
                App.USBEventType = USBEvent.Disconnected;
                if (!string.IsNullOrEmpty(App.DeviceVID) && !string.IsNullOrEmpty(App.DevicePID))
                {
                    // Resets Port status Screenshot
                    ApplicationMethods.resetPortStatus(App.CurrentPortStatus);
                    USBClassLibrary_Methods.GetCurrentPortStatus(App, portConnectionViewModel);
                    string DisconnectedPort = ApplicationMethods.GetPort(App);
                    if (!string.IsNullOrEmpty(DisconnectedPort))
                    {
                        App.NumberOfConnectedDevice--;
                        ApplicationMethods.ClearFields(DisconnectedPort, App.NumberOfConnectedDevice, portConnectionViewModel);
                        // clear datepicker
                        clearDatepicker(DisconnectedPort);
                    }



                }
            }

        }

        #endregion

        #region Printlabel container
        /// <summary>
        /// Function that invokes print function 
        /// </summary>
        /// <param name="elementName">name of button which the function is being called on to determine which device to print</param>
        public void printLabel(string elementName)
        {

            PrintLabel p = new PrintLabel();
            // Checks the textbox name for which device object to check and save
            if (elementName.Contains("1"))
            {
                p.PrintUsingFlowDocument(portConnectionViewModel.Device_1);
            }
            else if (elementName.Contains("2"))
            {
                p.PrintUsingFlowDocument(portConnectionViewModel.Device_2);
            }
            else if (elementName.Contains("3"))
            {
                p.PrintUsingFlowDocument(portConnectionViewModel.Device_3);
            }
        }

        #endregion

        #region Save device container

        /// <summary>
        /// Invoices the save data function 
        /// </summary>
        /// <param name="buttonName">name of button where the function is being invoked on</param>
        public void SaveDeviceInfo(string buttonName)
        {
            // Checks the textbox name for which device object to check and save
            if (buttonName.Contains("1"))
            {
                ApplicationMethods.SaveData(portConnectionViewModel.Device_1);

            }
            else if (buttonName.Contains("2"))
            {
                ApplicationMethods.SaveData(portConnectionViewModel.Device_2);

            }
            else if (buttonName.Contains("3"))
            {
                ApplicationMethods.SaveData(portConnectionViewModel.Device_3);

            }
        }

        #endregion

        #region set button content/headers to print label

        /// <summary>
        /// Sets button and headers to print label
        /// </summary>
        /// <param name="buttonName"></param>
        public void SetButtonMode(string buttonName, string type)
        {
            if (buttonName.Contains("1"))
            {
                // Enable the button
                portConnectionViewModel.Port1_Button = true;
                // Sets button content to Print Label                
                portConnectionViewModel.Port1_ButtonContent = type;
                ApplicationMethods.setHeaders(portConnectionViewModel.Device_1, PortStatusIconPath.PrinterIcon, type);

            }
            else if (buttonName.Contains("2"))
            {
                // Enable the button
                portConnectionViewModel.Port2_Button = true;
                // Sets button content to Print Label
                portConnectionViewModel.Port2_ButtonContent = type;
                ApplicationMethods.setHeaders(portConnectionViewModel.Device_2, PortStatusIconPath.PrinterIcon, type);

            }
            else if (buttonName.Contains("3"))
            {
                // Enable the button
                portConnectionViewModel.Port3_Button = true;
                // Sets button content to Print Label
                portConnectionViewModel.Port3_ButtonContent = type;
                ApplicationMethods.setHeaders(portConnectionViewModel.Device_3, PortStatusIconPath.PrinterIcon, type);


            }
        }

        #endregion

        #region Enable invoice date

        /// <summary>
        /// Enables Invoice date datepicker
        /// </summary>
        public void EnableInvoiceDate(string elementName, bool type)
        {        


            if (elementName.Contains("1"))
            {
                portConnectionViewModel.Port1_InvoiceDate = type;
            }
            else if (elementName.Contains("2"))
            {
                portConnectionViewModel.Port2_InvoiceDate = type;

            }
            else if (elementName.Contains("3"))
            {
                portConnectionViewModel.Port3_InvoiceDate = type;
            }
        }

        #endregion

        

        #region Event Handlers

        /// <summary>
        /// Button Handler 
        /// Modes : content = "Save device info" = push device object to php endpoint to save info
        /// content = "Print Label" = prints device object label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.USBEventType == USBEvent.Connected)
            {

                // Gets current button
                Button current = (Button)sender;
                string currentName = current.Name;
                string type = current.Content.ToString();
                //printLabel(currentName);

                if (type == "Save device info")
                {
                    // Save mode, pushes Device properties to php endpoint to save to database, generate csv and push into ftp
                    // if successful returns CSV Uploaded status which triggers means data succesfully saved and triggers the print mode
                    SaveDeviceInfo(currentName);

                    //MessageBox.Show("Saving device info");
                    //ApplicationMethods.SetDeviceProperty("Status", portConnectionViewModel, currentName, "CSV Uploaded");

                }
                else if (type == "Print Label")
                {
                    // Print mode prints device object properties into label
                    printLabel(currentName);

                    //MessageBox.Show("Printing");

                }



            }      

        }

        

        
       /// <summary>
       /// When Jobnumber is generated, check if it is valid. If valid enable Invoice date invoice datepicker to generate warranty status
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void Jobnumber_TextChangedHandler(object sender, TextChangedEventArgs e)
        {


            if (App.USBEventType == USBEvent.Connected)
            {
                TextBox current = (TextBox)sender;
                string jobnumber = current.Text;
                string currentName = current.Name;
                int jobnumberLength = jobnumber.Length;

                //Check if jobnumber is valid, if valid enable date invoice datepicker to generate warranty status
                if (jobnumberLength == 10)
                {
                    //Enable corresponding date invoice datepicker
                    EnableInvoiceDate(currentName, true);
                }
                else
                {
                    MessageBox.Show("Something went wrong contact data Department, Disconnect device");
                    //disable corresponding date invoice datepicker
                    EnableInvoiceDate(currentName, false);
                }
            }

            

        }

        /// <summary>
        /// Print Label When Status is set to CSV Uploaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Status_TextChangedHandler(object sender, TextChangedEventArgs e)
        {
            // Scenarios: Ready to save = Sets button to save mode, change button content// Headers to Save device info

            if (App.USBEventType == USBEvent.Connected)
            {
                // Gets Textbox Name
                TextBox current = (TextBox)sender;
                string currentName = current.Name;
                string statusContent = current.Text;

                //Enables the print label button

                if (statusContent.Trim() == "CSV Uploaded")
                {
                    // Set the button/header to print mode
                    SetButtonMode(currentName, "Print Label");

                }
                else if (statusContent.Trim() == "Ready to save")
                {
                    // change button mode to save mode
                    SetButtonMode(currentName, "Save device info");
                }
               
                
            }
        }

        /// <summary>
        /// Selected Change handler on the datepicker to calculate warranty status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WarrantyCheck(object sender, SelectionChangedEventArgs e)
        {
            if (App.USBEventType == USBEvent.Connected)
            {
                // Gets current Datepicker
                DatePicker current = (DatePicker)sender;
                string currentName = current.Name;

                ApplicationMethods.SetDeviceProperty("InvoiceDate", portConnectionViewModel, currentName, current.SelectedDate.Value.Date.ToShortDateString());

                // Subtract datepicker selected date from current date
                DateTime start = current.SelectedDate.Value.Date;
                DateTime finish = DateTime.Now;
                TimeSpan difference = finish.Subtract(start);
                string warrantyStatus;
                string warrantyCode;

                // IF date difference is  less than or equal to 365 device is in warranty if not out of warrant
                if (difference.TotalDays <= 365)
                {
                    warrantyStatus = "In Warranty";
                    warrantyCode = "7";
                }
                else
                {
                    warrantyStatus = "Out of warranty";
                    warrantyCode = "4";
                }
                // Sets warranty obj property 
                ApplicationMethods.SetDeviceProperty("Warranty", portConnectionViewModel, currentName, warrantyStatus);
                ApplicationMethods.SetDeviceProperty("WarrantyCode", portConnectionViewModel, currentName, warrantyCode);
            }

            
        }


        
        /// <summary>
        /// When Warranty Status changes to In Warranty/Out of warranty enable the button and put it in save mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WarrantyStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.USBEventType == USBEvent.Connected)
            {
                // Gets Textbox Name
                TextBox current = (TextBox)sender;
                string currentName = current.Name;
                string currentContent = current.Text;

                if (currentContent == "In Warranty" || currentContent == "Out of warranty")
                {
                    // set status to Ready to save
                    ApplicationMethods.SetDeviceProperty("Status", portConnectionViewModel, currentName, "Ready to save");
                }
            }

            
        }
        #endregion

        #region clear datepicker

        public void clearDatepicker(string disconnectedPort)
        {
            switch (disconnectedPort)
            {
                case "1":
                    // Resets datepicker
                    InvoiceDate_1.SelectedDate = null;

                    break;

                case "2":
                    // Resets datepicker
                    InvoiceDate_2.SelectedDate = null;
                    break;

                case "3":
                    // Resets datepicker
                    InvoiceDate_3.SelectedDate = null;
                    break;
            }
        }
        #endregion

    }
}

