using System;
using System.Configuration;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectEventWrappers;
using KinectProjectControllers;

namespace KinectWPFGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string saveFilePath = @"C:\Code\dataFile.txt";
        private WriteableBitmap bmpColor;
        private WriteableBitmap bmpDepth;
        private KinectProjectController kProjCtrlr;

        //Set true for saving data to a file, showing images, displaying extra data in lboxEvents.
        public bool showTestData;

        //Set true for sitting instead of standing
        public bool sitting { get; set; }

        //Set true for users placed close to the camera
        public bool closeRange { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            kProjCtrlr = new KinectProjectController(new KinectEventWrapper());

            //Subscribe to the closing event to handle unmanaged objects (ie. the KinecSensor)
            Closing += MainWindow_Closing;

            //Initialize booleans set by the GUI
            showTestData = chkboxShowTestData.IsChecked == true;
            sitting = chkboxSeated.IsChecked == true;
            closeRange = chkboxClose.IsChecked == true;

            SetSitStand();
            SetCloseFar();

            bmpColor = new WriteableBitmap(640, 480, 96.0, 96.0, PixelFormats.Bgr32, null);
            bmpDepth = new WriteableBitmap(640, 480, 96.0, 96.0, PixelFormats.Bgr32, null);
            imgColor.Source = bmpColor;
            imgDepth.Source = bmpDepth;
        }

        //Toggles setting the sensor between close and far
        private void SetCloseFar()
        {
            if (closeRange)
            {
                kProjCtrlr.ReceivedRequest(ReceivedRequestEnum.close);
            }
            else
            {
                kProjCtrlr.ReceivedRequest(ReceivedRequestEnum.far);
            }
        }

        //Toggles sitting/standing.
        private void SetSitStand()
        {
            if (sitting)
            {
                kProjCtrlr.ReceivedRequest(ReceivedRequestEnum.sit);
            }
            else
            {
                kProjCtrlr.ReceivedRequest(ReceivedRequestEnum.stand);
            }
        }

        private void btnClose_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            kProjCtrlr.Close();
        }

        private void LogError(Exception ex)
        {
            WriteTolboxEvents(ex.Message);
        }

        private void WriteTolboxEvents(string msg)
        {
            if (lboxEvents != null)
            {
                lboxEvents.Dispatcher.Invoke(new Action(() =>
                {
                    lboxEvents.Items.Add(msg);
                    lboxEvents.UpdateLayout();
                    lboxEvents.ScrollIntoView(lboxEvents.Items[lboxEvents.Items.Count - 1]);
                }));
            }
        }

        private void WriteToColorImage(byte[] imageData)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                bmpColor.WritePixels(new Int32Rect(0, 0, bmpColor.PixelWidth, bmpColor.PixelHeight), imageData, bmpColor.PixelWidth * sizeof(int), 0);
            }));
        }

        private void WriteToDepthImage(byte[] imageData)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                bmpDepth.WritePixels(new Int32Rect(0, 0, bmpColor.PixelWidth, bmpColor.PixelHeight), imageData, bmpColor.PixelWidth * sizeof(int), 0);
            }));
        }

        private void WriteFileData(byte[] data)
        {
            FileInfo fInfo = new FileInfo(saveFilePath);

            DirectoryInfo dInfo = fInfo.Directory;

            if (!dInfo.Exists)
            {
                dInfo.Create();
            }

            if (!fInfo.Exists)
            {
                fInfo.Create();
            }

            using (FileStream fWriter = fInfo.OpenWrite())
            {
                fWriter.Write(data, 0, data.Length);
            }
        }

        private void chkboxShowTestData_Checked(object sender, RoutedEventArgs e)
        {
            showTestData = true;
        }

        private void chkboxShowTestData_Unchecked(object sender, RoutedEventArgs e)
        {
            showTestData = false;
        }

        private void chkboxSeated_Checked(object sender, RoutedEventArgs e)
        {
            SetSitStand();
        }

        private void chkboxSeated_Unchecked(object sender, RoutedEventArgs e)
        {
            SetSitStand();
        }

        private void chkboxClose_Checked(object sender, RoutedEventArgs e)
        {
            closeRange = true;
            SetCloseFar();
        }

        private void chkboxClose_Unchecked(object sender, RoutedEventArgs e)
        {
            closeRange = false;
            SetCloseFar();
        }
    }
}
