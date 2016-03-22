using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace QuazarBarcodeScanner
{
    class MyFileManager
    {
        static Windows.Storage.StorageFolder storageFolder;
        static Windows.Storage.StorageFile barcodeFile;

        async public static Task SetTheBarcodeFile()
        {
            storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            barcodeFile = await storageFolder.CreateFileAsync("barcodes.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(barcodeFile, "In barcodeFile: ");
        }

        public static bool DoesBarcodeFileExist()
        {
            if (Object.Equals(barcodeFile, default(StorageFile)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        async public static Task<string> ReadOutBarcodeFile()
        {
            string barcodesFromFile = await FileIO.ReadTextAsync(barcodeFile);
            return barcodesFromFile;
        }
        async public static Task WriteToBarcodeFile(string newBarcode)
        {
            newBarcode = newBarcode + ", ";
            await FileIO.AppendTextAsync(barcodeFile, newBarcode);
        }
        async public static Task EmptyBarcodeFile()
        {
            await FileIO.WriteTextAsync(barcodeFile, "");
        }
    }
    
        
         
}
