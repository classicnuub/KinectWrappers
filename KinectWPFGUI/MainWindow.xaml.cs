using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectProjectControllers;

namespace KinectWPFGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string MemoryMappedFileName = "kinectData";
        private const string MutexName = "kinectMutex";
        private Timer updateTimer;
        private TimerCallback updateTimerCallback;
        private MemoryMappedFile mmf;
        private Mutex mutex;
        private string saveFilePath = @"C:\Code\dataFile.txt";
        private const long lng = 2458624;
        private WriteableBitmap bmpColor;
        private WriteableBitmap bmpDepth;
        private long timerTick = 10;

        //Set true for saving data to a file, showing images, displaying extra data in lboxEvents.
        public bool showTestData;

        //Set true for sitting instead of standing
        public bool sitting { get; set; }

        //Set true for users placed close to the camera
        public bool closeRange { get; set; }

        public MainWindow()
        {
            InitializeComponent();

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
            
            try
            {
                //Create the memory mapped file.
                mmf = MemoryMappedFile.CreateNew(MemoryMappedFileName, lng);
                
                lboxEvents.Items.Add("Created the " + MemoryMappedFileName + " memory mapped file.");
                bool mutexCreated;

                //Create the Mutex
                mutex = new Mutex(true, MutexName, out mutexCreated);
                if (mutexCreated)
                {
                    lboxEvents.Items.Add("Created the " + MutexName + " mutex.");
                    mutex.ReleaseMutex();

                    //Start the timer for polling the KinectSensor data
                    updateTimerCallback = updateTimer_Callback;
                    updateTimer = new Timer(updateTimerCallback, null, 0, timerTick);
                }
                else
                {
                    WriteTolboxEvents("The mutex was not created.  The program will not proceed.");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        //Toggles setting the sensor between close and far
        private void SetCloseFar()
        {
            if (closeRange)
            {
                KinectProjectController.ReceivedRequest(ReceivedRequestEnum.close);
            }
            else
            {
                KinectProjectController.ReceivedRequest(ReceivedRequestEnum.far);
            }
        }

        //Toggles sitting/standing.
        private void SetSitStand()
        {
            if (sitting)
            {
                KinectProjectController.ReceivedRequest(ReceivedRequestEnum.sit);
            }
            else
            {
                KinectProjectController.ReceivedRequest(ReceivedRequestEnum.stand);
            }
        }

        private void updateTimer_Callback(Object obj)
        {
            WriteMemoryData();
        }

        private void btnClose_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            KinectProjectController.Close();
            if (mmf != null)
            {
                mmf.Dispose();
            }
        }

        private void LogError(Exception ex)
        {
            WriteTolboxEvents(ex.Message);
        }

        /// <summary>
        /// Write the KinectData from the KinectEventWrappers object to the MemoryMappedFile
        /// </summary>
        private void WriteMemoryData()
        {
            try
            {
                byte[] response = KinectProjectController.ReceivedRequest("all");

                //Wait for control of the Mutex
                mutex.WaitOne();

                //Get a stream of the MemoryMappedFile
                using (MemoryMappedViewStream mmStrm = mmf.CreateViewStream())
                {
                    //Write to the MemoryMappedFile
                    BinaryWriter bWrite = new BinaryWriter(mmStrm);
                    bWrite.Write(response);
                }

                //Release the Mutex
                mutex.ReleaseMutex();

                //For debugging purposes extra information can be sent to the C# GUI to see what is being placed in the MemoryMappedFile
                if (showTestData == true)
                {
                    //Write the file data only if it's the full length data
                    if (response.Length == 2458624)
                    {
                        WriteFileData(response);
                    }

                    //Enable this method to test data in the C# code.
                    WriteTestingData();
                }

                WriteTolboxEvents("Updated memory data. Data length: " + response.Length);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Reads from the MemoryMappedFile and shows the results in the GUI
        /// </summary>
        private void WriteTestingData()
        {
            byte[] data = new byte[lng];

            //Get the data to display for testing from the memorymappedfile.
            mutex.WaitOne();
            using (MemoryMappedViewStream mmStrm = mmf.CreateViewStream())
            {
                BinaryReader bRead = new BinaryReader(mmStrm);
                bRead.Read(data, 0, Convert.ToInt32(lng));
            }
            mutex.ReleaseMutex();

            //Get the data to display from a file.
            //FileInfo fInfo = new FileInfo(saveFilePath);

            //if (!fInfo.Exists)
            //{
            //    using (FileStream fStream = fInfo.OpenRead())
            //    {
            //        fStream.Read(data, 0, Convert.ToInt32(lng));
            //    }
            //}

            byte[] skelData = new byte[1024];
            byte[] colorData = new byte[1228800];
            byte[] depthData = new byte[1228800];

            //Display color data in the image
            Buffer.BlockCopy(data, 0, colorData, 0, colorData.Length);
            WriteToColorImage(colorData);

            //Display depth data in the image.
            Buffer.BlockCopy(data, colorData.Length, depthData, 0, depthData.Length);
            WriteToDepthImage(depthData);

            //Write the skeleton data to the listbox
            Buffer.BlockCopy(data, 2457600, skelData, 0, 1024);
            string skelString = System.Text.Encoding.ASCII.GetString(skelData);
            WriteTolboxEvents(skelString);
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
            sitting = true;
            SetSitStand();
        }

        private void chkboxSeated_Unchecked(object sender, RoutedEventArgs e)
        {
            sitting = false;
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
