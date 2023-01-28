using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using OCR1.Backend.Converters;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OCR1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the stored values. Ex: enable console?
        /// </summary>
        private void ReadStoredSettings()
        {
            //get local settings folder
            //check exist, if not set default

            //names:
            //sett_log => false = no edit
            //sett_db
            List<string> sett_names = new List<string>();
            sett_names.Add("sett_log");
            sett_names.Add("sett_db");

            try
            {
                if (localSettings.Values[sett_names[0]] != null)
                {
                    //sett_log exists
                    if ((localSettings.Values[sett_names[0]].ToString() == "True") || (localSettings.Values[sett_names[0]].ToString() == "true"))
                    {
                        log_edit_tggl.Visibility = Visibility.Visible;
                        log_clear.Visibility = Visibility.Visible;
                        log_edit_tggl.IsChecked = true;
                    }

                    else
                    {
                        log_edit_tggl.IsChecked = false;
                        log_edit_tggl.Visibility = Visibility.Collapsed;
                        log_clear.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception e)
            {
                localSettings.Values["universal_log"] += "Error: " + e.ToString() + "\n";
            }
            try
            {
                if (localSettings.Values["universal_log"] != null)
                {
                    //sett_log exists
                    _console.Document.Selection.SetText(TextSetOptions.None, localSettings.Values["universal_log"].ToString());
                }
                else
                {
                    localSettings.Values["universal_log"] = "";
                }
            }
            catch (Exception e)
            {
                localSettings.Values["universal_log"] += "Error:" + e.ToString() + "\n" + "From: " + e.Source + "\n";
            }
        }
               
        SoftwareBitmap softwareBitmap;
        string bitmap_path = "";

        private async void upload_image(object sender, RoutedEventArgs e)
        {
            upload_btn_icon.Text = "\xE895";
            upload_btn_text.Text = "Proccessing...";

            img_preview_progress.IsActive = true;
            img_preview_status.Text = "Loading preview...";

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            //picker.FileTypeFilter.Add(".pdf");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file

                //save image
                BitmapImage bitmap = new BitmapImage(new Uri(file.Path, UriKind.Absolute));

                WriteableBitmap writeableBmp = BitmapFactory.New(512, 512);
                //ImageControl.Source = writeableBmp;
                using (writeableBmp.GetBitmapContext())
                {

                    // Load an image from the calling Assembly's resources via the relative path
                    writeableBmp = BitmapFactory.New(1, 1).FromResource("Data/flower2.png");
                }

                await SaveBitmapToFileAsync(writeableBitmap, "try1");

                //read from storage
                ImageSource source = await get_Image(file);                

                // Set the source of the Image control
                img_preview_progress.IsActive = false;
                img_preview_status.Text = "Nothing to show here";
                preview_image.Source = source;
                image_load.Visibility = Visibility.Collapsed;

                await Task.Delay(100);
                upload_btn_icon.Text = "\xE73E";
                upload_btn_text.Text = "Success";

                await Task.Delay(1500);
                upload_btn_icon.Text = "\xE74a";
                upload_btn_text.Text = "Upload";
            }
            else
            {
                //fail

                img_preview_progress.IsActive = false;
                img_preview_status.Text = "Preview failed";

                await Task.Delay(100);
                upload_btn_icon.Text = "\xE711";
                upload_btn_text.Text = "Fail";

                await Task.Delay(1500);
                upload_btn_icon.Text = "\xE74a";
                upload_btn_text.Text = "Upload";
                img_preview_status.Text = "Nothing to show here";
            }
        }

        public static async Task<ImageSource> get_Image(StorageFile sf)
        {
            using (var randomAccessStream = await sf.OpenAsync(FileAccessMode.Read))
            {
                var result = new BitmapImage();
                await result.SetSourceAsync(randomAccessStream);
                return result;
            }
        }

        public static async Task SaveBitmapToFileAsync(WriteableBitmap image, string name)
        {
            StorageFolder pictureFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProfilePictures", CreationCollisionOption.OpenIfExists);
            var file = await pictureFolder.CreateFileAsync(name + ".bmp", CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenStreamForWriteAsync())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream.AsRandomAccessStream());
                var pixelStream = image.PixelBuffer.AsStream();
                byte[] pixels = new byte[image.PixelBuffer.Length];

                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)image.PixelWidth, (uint)image.PixelHeight, 96, 96, pixels);

                await encoder.FlushAsync();
            }
        }

        //all global objects and variables
        #region helpers
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //run on load connections
            ReadStoredSettings();
        }

        private void Log_clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (localSettings.Values["universal_log"] != null)
                {
                    //sett_log exists
                    _console.Document.SetText(TextSetOptions.None, "");
                    localSettings.Values["universal_log"] = "";
                }
                else
                {
                    localSettings.Values["universal_log"] = "";
                }
            }
            catch
            {

            }
        }

        private void Log_copy_Click(object sender, RoutedEventArgs e)
        {
            string txt;
            _console.Document.GetText(TextGetOptions.None, out txt);
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(txt);
            Clipboard.SetContent(dataPackage);
        }

        private void _console_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                //sett_log exists
                string log_content = "";
                RichEditBox editBox = (RichEditBox)sender;
                editBox.Document.GetText(TextGetOptions.None, out log_content);
                localSettings.Values["universal_log"] = log_content;
            }
            catch(Exception ex)
            {
                _output.Document.Selection.SetText(TextSetOptions.None, ex.ToString());
            }
        }

        private async void Out_start_Click(object sender, RoutedEventArgs e)
        {

            //var btmp = Converters.converts(softwareBitmap);
            Bitmap bitmap = new Bitmap(bitmap_path);
            var v = Backend.ImagePrep.Digitalize.digitalize(bitmap);
            var w = Backend.ImagePrep.Cleanup.cleanup(v, bitmap.Width, bitmap.Height, 5);
            var b = Backend.ImagePrep.Transform.imageFromArray(w.Item1, bitmap.Width, bitmap.Height);

           // b.Save("C:\\Users\\filip\\Desktop\\b_try.png");

            try
            {
                //var class_ = new Backend.ANN_Interpreter.Interpreter();
                //var interp = class_.r;
                //var v = Backend.ImagePrep.Digitalize.digitalize(sftbitmap);
                //var w = Backend.ImagePrep.Cleanup.cleanup(v, softwareBitmap.PixelWidth, softwareBitmap.PixelHeight,5);
                //var b = Backend.ImagePrep.Transform.imageFromArray(w.Item1, sftbitmap.Width, sftbitmap.Height);

                //b.Save(localFolder.Path+"/try1.png");

                //localSettings.Values["universal_log"] += "v:" + v.ToString();

                
            }
            catch(Exception ex)
            {
                localSettings.Values["universal_log"] = "Error: " + ex;
            }

            ReadStoredSettings();
        }
    }
}
