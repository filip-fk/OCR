using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;

namespace OCR1.Backend.ImageRecog
{
    class ANN_Functions
    {
        /// <summary>
        /// The Sigmoid Function returns a value between 0 and 1, no matter how big the Input.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Pow(Math.E, -1 * x));
        }
        /// <summary>
        /// The Step Function returns a 1 whenever x is bigger or equal h.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static int BinaryStep(double x, double h)
        {
            if (x >= h)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// Returns Saved Data of all Layers and their Neurons
        /// </summary>
        /// <returns></returns>
        public List<List<List<double>>> readAllData()
        {
            try
            {
                //Try to open the File and Repeat Reading the Data until the End of the File
                StreamReader sr = new StreamReader("%userprofile%\\AppData\\Roaming\\OCR1\\Training\\network.ann");
                List<List<List<double>>> ANN_read = new List<List<List<double>>>();

                string line = sr.ReadLine();
                while (line != null)
                {
                    //Seperate each Layer into individual Neurons, devided by |
                    List<List<double>> Layer = new List<List<double>>();
                    string[] Neurons = line.Split("|");
                    foreach (string neuron in Neurons)
                    {
                        string[] values = neuron.Split(" ");
                        List<double> valuesDBL = new List<double>();
                        foreach (string val in values)
                        {
                            valuesDBL.Add(Convert.ToDouble(val));
                        }
                        Layer.Add(valuesDBL);
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
                return ANN_read;
            }
            catch
            {
                //In case one of the Above processes fails
                throw new Exception("An error has occured while reading values");
            }
        }
        /// <summary>
        /// Returns Saved Data of all Neurons in a Layer
        /// </summary>
        /// <param name="Layer">Layer Index</param>
        /// <returns></returns>
        public List<List<double>> readLayerData(int Layer)
        {
            return readAllData()[Layer];
        }
        /// <summary>
        /// Returns Saved Data of a Neuron
        /// </summary>
        /// <param name="Layer">Layer Index</param>
        /// <param name="Index">Neuron Index</param>
        /// <returns></returns>
        public List<double> readNeuronData(int Layer, int Index)
        {
            return readAllData()[Layer][Index];
        }

        public static int sayHi()
        {
            //
            #region helpers
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            #endregion
            localSettings.Values["universal_log"] = "";
            localSettings.Values["universal_log"] += "An error occurred. Error Code x0045d";
            return 0;
        }

        public double cost(double result, double target)
        {
            //TODO
            return 0.0;
        }
    }
}
