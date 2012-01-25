using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Phone;

namespace GEETHREE.DataClasses
{
    public class FileMaster
    {
        Object lockpad = new Object();

        /// <summary>
        /// Function to save avatar image to isolated storage
        /// </summary>
        public void saveImageToFile(Stream imageStream, string fileName)
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                lock (lockpad)
                {
                    if (myIsolatedStorage.FileExists(fileName))
                    {
                        myIsolatedStorage.DeleteFile(fileName);
                    }
                    IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(fileName);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(imageStream);
                    WriteableBitmap wb = new WriteableBitmap(bitmap);
                    wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85); fileStream.Close();
                }
            }
        }

        /// <summary>
        /// Function to read avatar image from isolated storage
        /// </summary>
        public WriteableBitmap readImageFromFile(string fileName)
        {
            WriteableBitmap bitmap = new WriteableBitmap(200, 200);
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                lock (lockpad)
                {
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        // Decode the JPEG stream.             
                        bitmap = PictureDecoder.DecodeJpeg(fileStream);
                    }
                }
            }
            return bitmap;
        }

    }
}
