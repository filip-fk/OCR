using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OCR1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
            ReadStoredSettings();
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
            //sett_db => false = no server connection

            List<string> sett_names = new List<string>();
            sett_names.Add("sett_log");
            sett_names.Add("sett_db");

            foreach(string sett_name in sett_names)
            {
                try
                {
                    if (localSettings.Values[sett_name] != null)
                    {
                        //sett_log exists
                        ToggleSwitch toggle = (ToggleSwitch)this.FindName(sett_name);
                        toggle.IsOn = Convert.ToBoolean(localSettings.Values[sett_name].ToString());
                    }
                    else localSettings.Values[sett_name] = "true";
                }
                catch (Exception e)
                {
                    localSettings.Values["universal_log"] += "Error:" + e.ToString() + "\n" + "From: " + e.Source + "\n";
                }
            }
        }

        /// <summary>
        /// Function called on each user interaction with toggle switch on settings page.
        /// saves new value to local settings
        /// </summary>
        /// <param name="sender">toggle switch of which the state (on/off) was changed</param>
        /// <param name="e"></param>
        private void Sett_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = (ToggleSwitch)sender;
            string toggle_name = toggle.Name;
            string toggle_value = toggle.IsOn.ToString();

            //save to local settings
            localSettings.Values[toggle_name] = toggle_value;
        }

        //all global objects and variables
        #region helpers
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        #endregion
    }
}
