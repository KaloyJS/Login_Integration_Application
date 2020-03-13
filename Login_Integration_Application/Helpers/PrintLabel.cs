using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing;


namespace Login_Integration_Application
{
    /// <summary>
    /// Class for printing Labels Passing Object properties in a FlowDocument
    /// </summary>
    public class PrintLabel
    {
        FlowDocument doc;
        PrintDialog printDlg = new PrintDialog();
        

        public PrintLabel()
        {
            doc = new FlowDocument();
            printDlg = new PrintDialog();
            
        }

        public void PrintUsingFlowDocument(Device obj)
        {            

            //We need to use paragraphs to addthe content in the blocks of flow document.  
            DateTime dateTime = DateTime.UtcNow.Date;

            string date = dateTime.ToString("MM/dd/yyyy");

            
            // Adding Jobnumber barcode
            BarCode(obj.Jobnumber);

            Paragraph p = new Paragraph();
            Span s = new Span();

            Span d = new Span();
            d = new Span(new Run(date + " "));
            d.Inlines.Add(new LineBreak());
            d.Inlines.Add(new Bold(new Run("USBPORT: ")));
            d.Inlines.Add(new Run(obj.Port + " "));
            d.Inlines.Add(new Bold(new Run(obj.WorkStation + " ")));
            d.Inlines.Add(new LineBreak());//Line break is used for next line.  
            p.Inlines.Add(d);

            // Location of connection
            //Span location = new Span();    
            //location.Inlines.Add(new Bold(new Run("USBPORT: ")));
            //location.Inlines.Add(new Run(port + " "));
            //location.Inlines.Add(new Run(workStation + " "));
            //location.Inlines.Add(new LineBreak());
            //p.Inlines.Add(location);

            // Jobnumber
            Span j = new Span();
            j.Inlines.Add(new Bold(new Run("JOBNUMBER: ")));
            j.Inlines.Add(new Run(obj.Jobnumber));
            j.Inlines.Add(new LineBreak());
            p.Inlines.Add(j);

            // Imei
            Span i = new Span();
            i.Inlines.Add(new Bold(new Run("IMEI: ")));
            i.Inlines.Add(new Run(obj.IMEI));
            i.Inlines.Add(new LineBreak());
            p.Inlines.Add(i);

            // Device model, color, capacity
            Span device = new Span();
            string model = obj.Model + " " + obj.Capacity + " " + obj.Color; 
            device.Inlines.Add(new Run(model));
            device.Inlines.Add(new LineBreak());
            p.Inlines.Add(device);

            // FMIP locked
            Span f = new Span();
            f.Inlines.Add(new Bold(new Run("FMIP LOCKED: ")));
            f.Inlines.Add(new Run(obj.FMIP));
            f.Inlines.Add(new LineBreak());
            p.Inlines.Add(f);

            // MDM Locked
            Span mdm = new Span();
            mdm.Inlines.Add(new Bold(new Run("MDM LOCKED: ")));
            mdm.Inlines.Add(new Run(obj.MDM_Lock));
            mdm.Inlines.Add(new LineBreak());
            p.Inlines.Add(mdm);

            //Give style and formatting to paragraph content.  
            p.FontFamily = new System.Windows.Media.FontFamily("Arial");
            p.FontSize = 12;
            p.FontStyle = FontStyles.Normal;
            p.TextAlignment = TextAlignment.Left;
            doc.Blocks.Add(p);
            //Print Image or barcode in flow document.  
            //let we need to print barcode for 123456.  

            doc.Name = "FlowDoc";
            doc.PageWidth = 400;
            doc.PagePadding = new Thickness(3, 5, 2, 4);
            // Create IDocumentPaginatorSource from FlowDocument
            IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer
            printDlg.PrintDocument(idpSource.DocumentPaginator, "Receipt Printing.");
            // Set headers to Label Printed
            ApplicationMethods.setHeaders(obj, PortStatusIconPath.SuccessIcon, "Label Printed");


        }

        public void BarCode(string stringForBarCode)
        {
            if (stringForBarCode != String.Empty)
            {
                string path = @"C:\\Login_Integration_Application\\BarCodeImages\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    System.IO.DirectoryInfo di = new DirectoryInfo(path);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }
                else
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(path);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();

                    }
                    string saveLocation = "C:\\Login_Integration_Application\\BarCodeImages\\" + Convert.ToString(stringForBarCode) + ".png"; //"/" + filename; \ 
                    GenerateImageString(Convert.ToString(Convert.ToString(stringForBarCode))).Save(saveLocation, ImageFormat.Png);
                    AddBarCode(saveLocation, "B");




                }
            }
        }

        //Convert string to barcode image.  
        private System.Drawing.Image GenerateImageString(string uniqueCode)
        {
            //Read in the parameters  
            string strData = uniqueCode;
            int imageHeight = 50;
            int imageWidth = 200;

            BarcodeLib.TYPE type = BarcodeLib.TYPE.UNSPECIFIED;
            type = BarcodeLib.TYPE.CODE128;
            System.Drawing.Image barcodeImage = null;
            try
            {
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                b.IncludeLabel = false;
                b.Alignment = BarcodeLib.AlignmentPositions.CENTER;
                barcodeImage = b.Encode(type, strData.Trim(), imageWidth, imageHeight);
                System.IO.MemoryStream MemStream = new System.IO.MemoryStream();
                barcodeImage.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = MemStream.ToArray();
                return byteArrayToImage(imageBytes);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                barcodeImage.Dispose();
            }
        }

        //Convert byte to image.  
        public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        //Add barcode to document.  
        public void AddBarCode(string ImagePath, string text)
        {
            Paragraph p = new Paragraph();
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            BitmapImage bimg = new BitmapImage();

            using (var stream = File.OpenRead(ImagePath))
            {
                bimg.BeginInit();
                bimg.CacheOption = BitmapCacheOption.OnLoad;
                bimg.StreamSource = stream;
                bimg.EndInit();
            }

            //bimg.BeginInit();
            //bimg.CacheOption = BitmapCacheOption.OnLoad;
            //bimg.UriSource = new Uri(ImagePath, UriKind.Absolute);
            //bimg.EndInit();
            image.Source = bimg;
            image.Width = 150;
            image.Height = 40;
            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            p.Inlines.Add(image);
            //p.Margin = new Thickness(0);
            //doc.Blocks.Add(new BlockUIContainer(image));
            doc.Blocks.Add(p);

        }
    }
}
