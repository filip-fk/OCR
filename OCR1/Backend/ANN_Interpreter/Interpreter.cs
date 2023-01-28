using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OCR1.Backend.ANN_Interpreter
{
    public class Interpreter
    {
        public Random RandObj = new Random();
        public class Keywords
        {

            /// <summary>
            /// Returns A Keyword's Possible Arguments.
            /// </summary>
            /// <param name="keyw">The Keyword</param>
            /// <returns></returns>
            public static List<string[]> PassClass(string keyw)
            {
                switch (keyw)
                {
                    case "Neuralnetwork":
                        return new List<string[]>() { Neuralnetwork.args1, Neuralnetwork.args2, Neuralnetwork.args3};
                    case "inputs":
                        return new List<string[]>() { inputs.args1 };
                    case "hiddenlayer":
                        return new List<string[]>() { hiddenlayer.args1 };
                    case "outputs":
                        return new List<string[]>() { outputs.args1 };
                    case "connection":
                        return new List<string[]>() { connection.args1 };
                    default:
                        throw new Exception("Keyword " + keyw + " does not exist.");
                }
            }

            public class KeywordInterface
            {
            }
            public class Neuralnetwork : KeywordInterface
            {
                public static string[] args1 = { "Sigmoid", "BinaryStep" };
                public static string[] args2 = { "Random" };
                public static string[] args3 = { "Backprop" };
            }
            public class inputs : KeywordInterface
            {
                public static string[] args1 = { "CONVERTTOINT" };
            }
            public class connection : KeywordInterface
            {
                public static string[] args1 = { "CONVERTTOINT" };
            }
            public class hiddenlayer : KeywordInterface
            {
                public static string[] args1 = { "CONVERTTOINT" };
            }
            public class outputs : KeywordInterface
            {
                public static string[] args1 = { "CONVERTTOINT" };
            }
        }

        
        
        //Structure
        //Ideas
        //There should be a way to define a Neural Network
        //Hidden Layers, the percentage of a Synapse spawning,
        //Input Layers, ...
        //The Program should read from the top down
        //Structure
        //Neuralnetwork:Type:WeightValues:Learning
        //inputs:NumberOfInputs
        //connection:probabilityOfASynapse
        //hiddenlayer:AmountNeurons
        //connections:""
        //outputs:NumberOfOutputs
        public static (List<string>, List<List<List<double>>>, bool) InterpretLine(string Line, string prevLine, List<string> NNSettings, List<List<List<double>>> Layers, int SynapseRate)
        {
            Random RandObj = new Random();
            bool finished = false;
            string lineKey = "";
            List<string> lineArgs = new List<string>();
            (lineKey, lineArgs) = GetLineArgs(Line);
            string prevLineKey = "";
            List<string> prevLineArgs = new List<string>();
            (prevLineKey, prevLineArgs) = GetLineArgs(prevLine);

            List<string[]> PossibleLineArgs = new List<string[]>();
            List<string[]> PossiblePrevLineArgs = new List<string[]>();

            PossibleLineArgs = Keywords.PassClass(lineKey);
            if (prevLineKey != null)
            {
                PossiblePrevLineArgs = Keywords.PassClass(prevLineKey);
                for (int i = 0; i < lineArgs.Count(); i++)
                {
                    if (PossibleLineArgs[i].Contains("CONVERTTOINT"))
                    {
                        if (lineKey == "inputs")
                        {
                            int inputNeurons = 0;
                            try
                            {
                                inputNeurons = Convert.ToInt32(lineArgs[0]);
                            }
                            catch
                            {
                                throw new Exception("Could not Convert " + lineArgs[0] + " to an amount of input Neurons");
                            }
                            if (NNSettings[1] == "Random")
                            {
                                //For Every input Neuron, Add a 1, so that the Program
                                //can Later Determine the amount of Input Neurons
                                for (int j = 0; i < inputNeurons; j++)
                                {
                                    Layers[0] = new List<List<double>> { };
                                    Layers[0][j].Add(1);
                                }
                            }
                        }
                        if (lineKey == "hiddenlayer")
                        {
                            int Neurons = 0;
                            try
                            {
                                Neurons = Convert.ToInt32(lineArgs[0]);
                            }
                            catch
                            {
                                throw new Exception("Could not Convert " + lineArgs[0] + " to an amount of hiddenlayer Neurons");
                            }
                            Layers[Layers.Count()] = new List<List<double>>() { };
                            for (int j = 0; j < Neurons; j++)
                            {
                                for (int k = 0; k < Layers[Layers.Count() - 1].Count() + 1; k++)
                                {
                                    Layers[Layers.Count() - 1][k].Add(RandObj.NextDouble());
                                }
                            }
                        }
                        if (lineKey == "outputs")
                        {
                            int outputNeurons = 0;
                            try
                            {
                                outputNeurons = Convert.ToInt32(lineArgs[0]);
                            }
                            catch
                            {
                                throw new Exception("Could not Convert " + lineArgs[0] + " to an amount of output Neurons");
                            }
                            Layers[Layers.Count()] = new List<List<double>>() { };
                            for (int j = 0; j < outputNeurons; j++)
                            {
                                Layers[Layers.Count() - 1][j].Add(1);
                            }
                            finished = true;
                        }
                    }
                }
            }
            else
            {
                if (lineKey == "Neuralnetwork")
                {
                    for (int i = 0; i < lineArgs.Count(); i++)
                    {
                        if (PossibleLineArgs[i].Contains(lineArgs[i]))
                        {
                            NNSettings.Add(lineArgs[i]);
                        }
                        else
                        {
                            throw new Exception("Argument " + lineArgs[i] + " Does not exsist in this context");
                        }
                    }
                }
            }

            //Returns The Settings, Layers, and finished status in a tuple
            return (NNSettings, Layers, finished);

        }
        /// <summary>
        /// Reads the Keyword and Arguments from the Line's Content
        /// </summary>
        /// <param name="Line">The Line Content</param>
        /// <returns></returns>
        public static (string, List<string>) GetLineArgs(string Line)
        {
            string keyword = "";
            List<string> args = new List<string>();
            if (Line != null)
            {
                string[] lineCont = Line.Split(":");
                for (int i = 0; i < lineCont.Count(); i++)
                {
                    //Set the Keyword to the Line's first Statement
                    if (i == 0)
                    {
                        keyword = lineCont[0];
                    }
                    //Add the Line's Arguments to the Arguments List
                    else
                    {
                        args.Add(lineCont[i]);
                    }
                }
                return (keyword, args);
            }
            else
            {
                return (null, null);
            }
        }
        /// <summary>
        /// Reads the Line and returns a List of all the lines
        /// </summary>
        /// <param name="file">File Path</param>
        /// <returns></returns>
        public static List<string> ReadFile(string file)
        {
            StreamReader sr = new StreamReader(file);

            string line = sr.ReadLine();
            int lineNum = 0;
            List<string> lines = new List<string>() { };
            //Continously Read Lines until File End
            while (line != null)
            {
                string prevLine = null;
                string lineKey = "";
                lines.Add(line);
                prevLine = line;
                line = sr.ReadLine();
                lineNum += 1;
            }
            sr.Close();
            return lines;
        }
        /// <summary>
        /// Creates a Neural Network File and its Config
        /// </summary>
        /// <param name="NNSettings">The Settings of the Neural Network</param>
        /// <param name="Layers">The Neural Network's Layers</param>
        public static void WriteFiles(List<string> NNSettings, List<List<List<double>>> Layers)
        {
            //Each Line is one Argument
            //Neuralnetwork:Type:WeightValues:Learning
            //Type - Sigmoid, BinaryStep
            //WeightValues - Random
            //Learning - Backprop
            StreamWriter cfg_sw = new StreamWriter("%userprofile%\\AppData\\Roaming\\OCR1\\Training\\config.cfg");
            foreach (string arg in NNSettings)
            {
                cfg_sw.WriteLine(arg);
            }
            cfg_sw.Close();

            //Each Layer is one Line
            //Each Neuron is devided by "|"
            //Each Neuron Data is devided by " "
            StreamWriter sw = new StreamWriter("%userprofile%\\AppData\\Roaming\\OCR1\\Training\\network.ann");
            for (int a = 0; a < Layers.Count(); a++)
            {
                //Do this for every Layer
                string LayerString = "";
                for (int b = 0; b < Layers[a].Count(); b++)
                {
                    //Do this for every Neuron
                    for (int c = 0; c < Layers[a][b].Count(); c++)
                    {
                        //Do this for every Value from the Neuron
                        LayerString += Layers[a][b][c];
                        if (c != Layers[a][b].Count() - 1)
                        {
                            LayerString += " ";
                        }
                    }
                    //Add only a Separation sign if it is not the
                    //Last Neuron of the Layer
                    if (b != Layers[a].Count() - 1)
                    {
                        LayerString += "|";
                    }
                }
                sw.WriteLine(LayerString);
            }
            sw.Close();
        }
        /// <summary>
        /// Runs the Interpreter and saves the Neural Network File
        /// </summary>
        public class Run
        {
            private List<string> NNSettings = new List<string>();
            private List<List<List<double>>> Layers = new List<List<List<double>>>();
            private int SynapseRate = 100;
            private int currentLineIndex = 0;
            private bool finished = false;
            public Run(string file)
            {
                string prevline = null;
                string line = "";
                List<string> lines = ReadFile(file); 
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (i == 0)
                    {
                        line = lines[i];
                    }
                    else
                    {
                        line = lines[i];
                        prevline = lines[i - 1];
                    }
                    (NNSettings, Layers, finished) = InterpretLine(line, prevline, NNSettings, Layers, SynapseRate);
                }
                WriteFiles(NNSettings, Layers);
            }
        }
        //public Run run_ = new Run();
    }
}