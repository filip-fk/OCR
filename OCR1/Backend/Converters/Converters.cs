using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Windows.Graphics.Imaging;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace OCR1.Backend.Converters
{
    class Converters
    {
        /// <summary>
        /// Returns a Bitmap with a SoftwareBitmap as input
        /// </summary>
        /// <param name="value">SoftwareBitmap to convert</param>
        /// <returns></returns>
        public static async Task<Bitmap> converts_SBtoB(SoftwareBitmap value)
        {
            //create file and folder
            string file_path = "";
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile tmpFile = await localFolder.CreateFileAsync("tmp_1.jpg", CreationCollisionOption.GenerateUniqueName);

            //write to the empty bitmap file
            using (IRandomAccessStream stream = await tmpFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an encoder with the desired format
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(value);
                await encoder.FlushAsync();

                try
                {
                    //await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                    switch (err.HResult)
                    {
                        case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                            // If the encoder does not support writing a thumbnail, then try again
                            // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            throw;
                    }
                }

                if (encoder.IsThumbnailGenerated == false)
                {
                    await encoder.FlushAsync();
                }
            }

            //returnnsaved file from file path
            file_path = tmpFile.Path;
            if (file_path != "")
            {
                Bitmap bmp = new Bitmap(file_path);

                return bmp;
            }
            else
            {
                return null;
            }
        }

        //private async void SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile outputFile)
        //{
        //    using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
        //    {
        //        // Create an encoder with the desired format
        //        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
        //
        //        // Set the software bitmap
        //        encoder.SetSoftwareBitmap(softwareBitmap);
        //        await encoder.FlushAsync();
        //
        //        try
        //        {
        //            //await encoder.FlushAsync();
        //        }
        //        catch (Exception err)
        //        {
        //            const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
        //            switch (err.HResult)
        //            {
        //                case WINCODEC_ERR_UNSUPPORTEDOPERATION:
        //                    // If the encoder does not support writing a thumbnail, then try again
        //                    // but disable thumbnail generation.
        //                    encoder.IsThumbnailGenerated = false;
        //                    break;
        //                default:
        //                    throw;
        //            }
        //        }
        //
        //        if (encoder.IsThumbnailGenerated == false)
        //        {
        //            await encoder.FlushAsync();
        //        }
        //
        //
        //    }
        //}

        //string file_path = "";
        //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        //private async void createTmpFile(SoftwareBitmap value)
        //{
        //    StorageFile tmpFile = await localFolder.CreateFileAsync("tmp_1.jpg", CreationCollisionOption.GenerateUniqueName);
        //    SaveSoftwareBitmapToFile(value, tmpFile);
        //    file_path = tmpFile.Path;
        //}


        /// <summary>
        /// Returns a SoftwareBitmap with a Bitmap as input
        /// </summary>
        /// <param name="value">Bitmap to convert</param>
        /// <returns></returns>
        public static async Task<SoftwareBitmap> converts(Bitmap value)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            value.Save(localFolder.Path + "\\tmp_1.png");
            SoftwareBitmap sftBmp;
            StorageFile tmpFile = await localFolder.GetFileAsync("tmp_1.png");
            using (IRandomAccessStream stream = await tmpFile.OpenAsync(FileAccessMode.Read))
            {
                // Create the decoder from the stream
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                // Get the SoftwareBitmap representation of the file
                sftBmp = await decoder.GetSoftwareBitmapAsync();
            }
            return sftBmp;
        }
    }
}
