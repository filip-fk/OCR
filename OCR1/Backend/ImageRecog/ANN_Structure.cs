using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OCR1.Backend.ImageRecog;

namespace OCR1.Backend.ImageRecog
{
    class ANN_Structure
    {
        class Neuron
        {
            /// <summary>
            /// Takes inputs and weights, and returns a Value
            /// </summary>
            /// <param name="inputs">Neuron Inputs</param>
            /// <param name="weights">Neuron Weights</param>
            /// <param name="bias">Neuron Bias</param>
            /// <param name="returnType">0 - Sigmoid Function; 1 - Binary Step</param>
            /// <param name="binaryStepH">Changes the Actuation of the Binary Step Function</param>
            /// <returns></returns>
            public static double predict(List<double> inputs, List<double> weights, double bias, int returnType, double binaryStepH = 0.5)
            {
                if (inputs.Count() == weights.Count())
                {
                    double result = 0;
                    for (int i=0; i < inputs.Count(); i++)
                    {
                        result += inputs[i] * weights[i];
                    }
                    result += bias;
                    if (returnType == 0)
                    {
                        //Runs if returnType is 0, or Sigmoid - makes Sigmoid Perceptron
                        return ImageRecog.ANN_Functions.Sigmoid(result);
                    }
                    else if (returnType == 1)
                    {
                        //Runs if returnType is 1, or Binary Step - Makes Perceptron
                        return ImageRecog.ANN_Functions.BinaryStep(result, binaryStepH);
                    }
                    else
                    {
                        //Runs if returnType is neither 0, nor 1. Throws an Error
                        throw new Exception(String.Format("Neuron Error - returnType {0} is unknown. Use 0 for Sigmoid, and 1 for Binary Step Function",returnType));
                    }
                    
                }
                else
                {
                    //Runs if the amount of inputs does not match the amount of weights. Throws an Error
                    throw new Exception(String.Format("Neuron Error - Number of Inputs ({0}) does not Match Number of Weights ({1})", inputs.Count(), weights.Count()));
                }
            }
            
        }
        class Layer
        {
            
            public static (List<List<double>>, List<double>, List<double>)  predict(List<List<double>> inputs, List<List<double>> weightsAll)
            {
                List<List<double>> weights = new List<List<double>>();
                List<double> bias = new List<double>();
                List<double> outputs = new List<double>();
                //Separate Bias and Weights
                for (int i = 0; i < weightsAll.Count(); i++)
                {
                    bias.Add(weightsAll[i].Last());
                    for (int j = 0; j < weightsAll[i].Count() - 1; j++)
                    {
                        weights[i].Add(weightsAll[i][j]);
                    }
                }
                //Let each Neuron Predict using the Bias and Weight values Sepatated above
                //And add their Predictions to an output List
                for (int i = 0; i < weightsAll.Count(); i++)
                {
                    outputs.Add(Neuron.predict(inputs[i], weights[i], bias[i], 0, 0.5));
                }
                //Return the Biases, Weights
                return (weights, bias, outputs);
            }
        }
        class Network
        {
            public Network ()
            {
                
            }
        }
    }
}
