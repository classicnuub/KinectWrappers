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
using Microsoft.Kinect;
using WindowsMemoryMappedFileNS;

namespace KinectWPFGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Declarations
        private WriteableBitmap bmpColor;
        private WriteableBitmap bmpDepth;
        private KinectProjectController kProjCtrlr;
        private WindowsMemoryMappedFile colorMMF;
        private WindowsMemoryMappedFile depthMMF;
        private Timer updateImagesTimer;
        private long updateIMagesTimerCallbackPeriod = 33;

        //Set true for saving data to a file, showing images, displaying extra data in lboxEvents.
        public bool showTestData;

        //Set true for sitting instead of standing
        public bool sitting { get; set; }

        //Set true for users placed close to the camera
        public bool closeRange { get; set; }
        #endregion Declarations

        public MainWindow()
        {
            colorMMF = 
                new WindowsMemoryMappedFile(KinectSettings.Default.ColorMutexName, KinectSettings.Default.ColorMemoryMappedFileName);
            depthMMF =
                new WindowsMemoryMappedFile(KinectSettings.Default.DepthMutexName, KinectSettings.Default.DepthMemoryMappedFileName);

            //This class does not necessarily need to be used here, but since this GUI is for testing it will remain.
            kProjCtrlr = new KinectProjectController(
                    new KinectEventWrapper(ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution640x480Fps30),
                    colorMMF,
                    depthMMF);

            updateImagesTimer = new Timer(updateImagesTimer_Callback, null, 0, updateIMagesTimerCallbackPeriod);

            InitializeComponent();

            //Subscribe to the closing event to handle unmanaged objects (ie. the KinecSensor)
            Closing += MainWindow_Closing;

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

        #region Methods
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

            closeRange = !closeRange;
        }

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

            sitting = !sitting;
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

        private void WriteToColorImage(byte[] imageData, int width, int height)
        {
            if (imageData.Length > 0)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    bmpColor.WritePixels(new Int32Rect(0, 0, width, height), imageData, width * sizeof(int), 0);
                }));
            }
        }

        private void WriteToDepthImage(byte[] imageData, int width, int height)
        {
            if (imageData.Length > 0)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    bmpDepth.WritePixels(new Int32Rect(0, 0, width, height), imageData, width * sizeof(int), 0);
                }));
            }
        }
        #endregion Methods

        #region Events
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            kProjCtrlr.Close();
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
            SetCloseFar();
        }

        private void chkboxClose_Unchecked(object sender, RoutedEventArgs e)
        {
            SetCloseFar();
        }

        private void kProjCtrlr_DepthImageChanged(byte[] bitmap, int width, int height)
        {
            WriteToDepthImage(bitmap, width, height);
        }

        private void kProjCtrlr_ColorImageChanged(byte[] bitmap, int width, int height)
        {
            WriteToColorImage(bitmap, width, height);
        }

        private void updateImagesTimer_Callback(Object obj)
        {
            WriteToColorImage(colorMMF.ReadFromDataSource(), 640, 480);
            WriteToDepthImage(depthMMF.ReadFromDataSource(), 640, 480);
        }
        #endregion Events
    }
}
