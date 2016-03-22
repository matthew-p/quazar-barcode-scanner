using System;
using System.Collections.Generic;
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
using ZXing.Mobile;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuazarBarcodeScanner
{
 
    public sealed partial class ControlPage : Page
    {
        // invoke the scanner object 
        MobileBarcodeScanner scanner;


        public ControlPage()
        {
            this.InitializeComponent();
            // create a new instance of the scanner
            scanner = new MobileBarcodeScanner(this.Dispatcher);
            scanner.Dispatcher = this.Dispatcher;
        }
        /* this is overriding the OnNavigatedTo method to fire events when the mainpage is loaded including asyncronous actions, 
        // async acts are placed here because the public MainPage() constructure cannot, and should not be async. 
        // see http://grogansoft.com/blog/?p=1028 
        // this is how you should do all the initial async IO on start
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // StorageFile newFile = await localFolder.CreateFileAsync(desiredName, CreationCollisionOption.FailIfExists);
            // passed the StorageFile barcodeFile made in onNavigation to MainPage here, cast it from generic object back 
            // to the StorageFile type
            // check to see if barcodeFile has been initialized yet or not 
            // http://stackoverflow.com/questions/1195597/how-to-tell-whether-a-variable-has-been-initialized-in-c
            /*
            if (Object.Equals(barcodeFile, default(StorageFile)))
            {
                barcodeFile = (StorageFile)e.Parameter;
            }
            
            OutputTextBlock.Text = "in onNavigatedTo";
            
        }
        */
        async private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            // set custom text instructions to user & scanning options, tryharder for different orientations 
            scanner.TopText = "Place Barcode in Front of the Camera";
            scanner.BottomText = "Touch Screen to Focus Camera";
            var scanningOptions = new MobileBarcodeScanningOptions();
            scanningOptions.TryHarder = true;

            // start scan Task
            var scannedCode = await scanner.Scan(scanningOptions);
            // OutputTextBlock.Text = "After await scanner.Scan ";
            await MyFileManager.WriteToBarcodeFile(scannedCode.Text);
            // await FileIO.AppendTextAsync(barcodeFile, scannedCode.Text);

            // OutputTextBlock.Text += " After await WriteToBarcodeFile ";
        }

        async private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            //string barcodesFromFile = await FileIO.ReadTextAsync(barcodeFile);
            string barcodesFromFile = await MyFileManager.ReadOutBarcodeFile();
            await MessageBox("Barcodes from file: \n" + barcodesFromFile);
        }

        private async void ComposeEmail(string messageBody)
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

        async private void ReadFirstBarcodeButton_Click(object sender, RoutedEventArgs e)
        {
            string fileContent = await MyFileManager.ReadOutBarcodeFile();
            
            OutputTextBlock.Text = "ReadFirst button, File contents: " + fileContent;
       
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
