﻿
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

        // Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
     
       
        public MainPage()
        {
            this.InitializeComponent();
        }
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            /*
            Windows.Storage.StorageFile barcodeFile = await storageFolder.CreateFileAsync("barcodes.txt", 
                Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await FileIO.AppendTextAsync(barcodeFile, "inside barcodeFile ");
            */
            if (!MyFileManager.DoesBarcodeFileExist())
            {
                await MyFileManager.SetTheBarcodeFile();
            }
            

            PrimaryFrame.Navigate(typeof(ControlPage));
        }

    }
}