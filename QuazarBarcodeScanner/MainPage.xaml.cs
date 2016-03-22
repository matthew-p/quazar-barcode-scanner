
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZXing;
using ZXing.Mobile;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QuazarBarcodeScanner
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class MainPage : Page
    {
        UIElement customOverlayElement = null;
        // invoke the scanner object 
        MobileBarcodeScanner scanner;

        public MainPage()
        {
            this.InitializeComponent();
            // create a new instance of the scanner
            scanner = new MobileBarcodeScanner(this.Dispatcher);
            scanner.Dispatcher = this.Dispatcher;
            /*
            if (customOverlayElement == null)
            {
                customOverlayElement = this.Overlay.Children[0];
                this.Overlay.Children.RemoveAt(0);
            }

            scanner.CustomOverlay = customOverlayElement;
            scanner.UseCustomOverlay = true;
            */
        }
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (!MyFileManager.DoesBarcodeFileExist())
            {
                await MyFileManager.SetTheBarcodeFile();
            }

            // PrimaryFrame.Navigate(typeof(ControlPage));
        }
        /*
        async private void ScanButton_Click(object sender, RoutedEventArgs e)
        {

            ButtonCancel.Click += (s, e2) => 
            {
                scanner.Cancel();
                ZXingScannerControl.
                
            };
            ButtonTorch.Click += (s, e2) =>
            {
                scanner.Torch(true);
                scanner.AutoFocus();
            };
            // set custom text instructions to user & scanning options, tryharder for different orientations 
            scanner.TopText = "Place Barcode in Front of the Camera";
            scanner.BottomText = "Touch Screen to Focus Camera";
            var scanningOptions = new MobileBarcodeScanningOptions();
            scanningOptions.TryHarder = true;

            // start scan Task
            var scannedCode = await scanner.Scan(scanningOptions);
            await MyFileManager.WriteToBarcodeFile(scannedCode.Text);

        }
        */
        private void buttonScanCustom_Click(object sender, RoutedEventArgs e)
        {
            //Get our UIElement from the MainPage.xaml (this) file 
            // to use as our custom overlay
            if (customOverlayElement == null)
            {
                customOverlayElement = this.Overlay.Children[0];
                this.Overlay.Children.RemoveAt(0);
            }

            //Wireup our buttons from the custom overlay
            this.ButtonCancel.Click += (s, e2) =>
            {
                scanner.Cancel();
            };
            this.ButtonTorch.Click += (s, e2) =>
            {
                scanner.ToggleTorch();
            };

            //Set our custom overlay and enable it
            scanner.CustomOverlay = customOverlayElement;
            scanner.UseCustomOverlay = true;
            var scanningOptions = new MobileBarcodeScanningOptions();
            scanningOptions.TryHarder = true;

            //Start scanning
            scanner.Scan(scanningOptions).ContinueWith(t =>
            {
                if (t.Result != null)
                    HandleScanResult(t.Result);
            });
        }

        async void HandleScanResult(ZXing.Result result)
        {
            string msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                string scannedCode = result.Text + ",\n\r";
                msg = "Found Barcode: " + result.Text;
                await MyFileManager.WriteToBarcodeFile(scannedCode);
            }

            else
                msg = "Scanning Canceled!";

            await MessageBox(msg);

        }
        async private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            string barcodesFromFile = await MyFileManager.ReadOutBarcodeFile();
            await ComposeEmail(barcodesFromFile);
        }

        private async Task ComposeEmail(string messageBody)
        {
            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
            emailMessage.Body = messageBody;

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        async private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string fileContents = "";
            fileContents = await MyFileManager.ReadOutBarcodeFile();
            if (!String.IsNullOrEmpty(fileContents))
            {
                var confirmBox = new MessageDialog("Are you sure you want to delete all stored barcodes?");
                confirmBox.Title = "Confirm";
                confirmBox.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
                confirmBox.Commands.Add(new UICommand { Label = "No", Id = 1 });

                var result = await confirmBox.ShowAsync();

                if ((int)result.Id == 0)
                {
                    await MyFileManager.EmptyBarcodeFile();
                }
            }
            else if (String.IsNullOrEmpty(fileContents))
            {
                await MessageBox("No barcodes stored to delete!");
            }

        }

        async private void ShowBarcodesButton_Click(object sender, RoutedEventArgs e)
        {
            string fileContent = await MyFileManager.ReadOutBarcodeFile();
            OutputTextBlock.Text = "ShowBarcodes button, File contents: " + fileContent;

        }

        async Task MessageBox(string text)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var dialog = new MessageDialog(text);
                await dialog.ShowAsync();
            });
        }

    }
}
