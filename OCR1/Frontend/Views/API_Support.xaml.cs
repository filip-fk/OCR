using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OCR1.Frontend.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class API_Support : Page
    {
        public API_Support()
        {
            this.InitializeComponent();
            this.RecObjModel = new Recognized_Object_Model();
        }
        public Recognized_Object_Model RecObjModel { get; set; }
        
        //file picker
        private async void Pick_File(object sender, RoutedEventArgs e)
        {
            progress_ring.IsActive = true;

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var files = await picker.PickMultipleFilesAsync();

            //set bitmap and text
            SoftwareBitmap _softwareBitmap = null;
            string _output = "Empty";

            if (files.Count > 0)
            {
                // Application now has read/write access to the picked file(s)
                foreach (Windows.Storage.StorageFile file in files)
                {
                    //convert file to sftBtmp
                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        // Create the decoder from the stream
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                        // Get the SoftwareBitmap representation of the file
                        _softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    }

                    //convert img to txt
                    OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                    OcrResult ocrResult = await ocrEngine.RecognizeAsync(_softwareBitmap);

                    string extractedText = ocrResult.Text;
                    _output = ocrResult.Text;

                    if (_softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || _softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
                    {
                        _softwareBitmap = SoftwareBitmap.Convert(_softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    }

                    SoftwareBitmapSource _softwareBitmapSource = new SoftwareBitmapSource();
                    await _softwareBitmapSource.SetBitmapAsync(_softwareBitmap);

                    RecObjModel.RecObjs.Add(new Recognized_Object { Bitmap = _softwareBitmapSource, Output = _output });
                }
            }
            else
            {
                //handle error... somehow....
                //_output = "Operation cancelled.";
            }
        }

        private void Copy_Result(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            TextBlock txtbl = (TextBlock)((Grid)btn.Parent).Children.ElementAt(1);

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(txtbl.Text);
            Clipboard.SetContent(dataPackage);

            try
            {
                //Button btn = (Button)sender;
                //TextBlock txtbl = (TextBlock)((Grid)btn.Parent).Children.ElementAt(1);
            }
            catch
            {
                localSettings.Values["universal_log"] += "Error: API_Support.Copy_Result() failed";
            }
        }

        #region helpers
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        #endregion

        private void Copy_All(object sender, RoutedEventArgs e)
        {
            string _temp = "";

            foreach(Recognized_Object obj in RecObjModel.RecObjs)
            {
                _temp+=obj.Output + " || ";
            }

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(_temp);
            Clipboard.SetContent(dataPackage);

            try
            {
                //Button btn = (Button)sender;
                //TextBlock txtbl = (TextBlock)((Grid)btn.Parent).Children.ElementAt(1);
            }
            catch
            {
                localSettings.Values["universal_log"] += "Error: API_Support.Copy_Result() failed";
            }
        }
    }

    public class Recognized_Object
    {
        public SoftwareBitmapSource Bitmap { get; set; }
        public string Output { get; set; }
        public int Status { get; set; }
        public string _Status
        {
            get
            {
                string temp = "Fail";
                switch (Status)
                {
                    case 0:
                        temp = "Fail";
                        break;
                    case 1:
                        temp = "Success";
                        break;
                }

                return temp;
            }
        }
    }

    public class Recognized_Object_Model
    {
        public ObservableCollection<Recognized_Object> RecObjs { get; } = new ObservableCollection<Recognized_Object>();
    }
}
