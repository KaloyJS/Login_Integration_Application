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

                    }



                }
            }

        }

        #endregion


        #region Event Handlers
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.USBEventType == USBEvent.Connected)
            {
                
                // Gets Textbox Name
                Button current = (Button)sender;
                string currentName = current.Name;

                printLabel(currentName);
                


            }
            
            
            

        }

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

        
        /// <summary>
        /// Push data to database when jobnumber property is set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Jobnumber_TextChangedHandler(object sender, TextChangedEventArgs e)
        {
            if (App.USBEventType == USBEvent.Connected)
            {
                // Gets Textbox Name
                TextBox current = (TextBox)sender;
                string currentName = current.Name;

                // Checks the textbox name for which device object to check and save
                if (currentName.Contains("1"))
                {
                    ApplicationMethods.SaveData(portConnectionViewModel.Device_1);

                }
                else if (currentName.Contains("2"))
                {
                    ApplicationMethods.SaveData(portConnectionViewModel.Device_2);

                }
                else if (currentName.Contains("3"))
                {
                    ApplicationMethods.SaveData(portConnectionViewModel.Device_3);

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
            if (App.USBEventType == USBEvent.Connected)
            {
                // Gets Textbox Name
                TextBox current = (TextBox)sender;
                string currentName = current.Name;
                string statusContent = current.Text;

                if (statusContent.Trim() == "CSV Uploaded")
                {
                    // Checks the textbox name for which device object to check and save
                    if (currentName.Contains("1"))
                    {
                        portConnectionViewModel.Port1_PrintButton = true;
                        ApplicationMethods.setHeaders(portConnectionViewModel.Device_1, PortStatusIconPath.PrinterIcon, "Print Label");
                    }
                    else if (currentName.Contains("2"))
                    {
                        portConnectionViewModel.Port2_PrintButton = true;
                        ApplicationMethods.setHeaders(portConnectionViewModel.Device_2, PortStatusIconPath.PrinterIcon, "Print Label");
                    }
                    else if (currentName.Contains("3"))
                    {
                        portConnectionViewModel.Port3_PrintButton = true;
                        ApplicationMethods.setHeaders(portConnectionViewModel.Device_3, PortStatusIconPath.PrinterIcon, "Print Label");

                    }
                }
               
                
            }
        }

        #endregion
    }
}

