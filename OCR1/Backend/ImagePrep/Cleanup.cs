using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCR1.Backend.ImagePrep;

namespace OCR1.Backend.ImagePrep
{
    class Cleanup
    {
        /// <summary>
        /// Returns a List of all Constellations in an image
        /// </summary>
        /// <param name="image">The Image to look for Constellations</param>
        /// <param name="imageWidth">The Image's Width</param>
        /// <param name="imageHeight">The Image's Height</param>
        /// <returns></returns>
        public static List<List<(int, int)>> GetConstellations(double[,] image, int imageWidth, int imageHeight)
        {

            List<List<(int, int)>> neighbors = new List<List<(int, int)>>();

            List<List<(int, int)>> constellations = new List<List<(int, int)>>();

            //Each Pixel's Index in the "neighbors" List is equal to (x * imgHeight) + y
            //Check every pixel for Neighbors and add all Neighbors to the "neighbors" List
            for (int x = 0; x < imageWidth; x++)
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    List<(int, int)> pxNeighbors = checkNeighbors(image, imageWidth, imageHeight, x, y);
                    List<(int, int)> noNeighbors = new List<(int, int)>();

                    if (pxNeighbors != noNeighbors)
                    {
                        neighbors.Add(pxNeighbors);
                    }
                }
            }

            for (int x = 0; x < neighbors.Count(); x++)
            {
                if (constellations.Count() == 0)
                {
                    constellations.Add(neighbors[x]);       
                }
                else
                {
                    bool hasConstellation = false;
                    //Look through own neighbours, and if one of them is in a Constellation
                    foreach ((int, int) val in neighbors[x])
                    {
                        //Look through Every existing Constellation
                        for (int y = 0; y < constellations.Count(); y++)
                        {
                            //Look if the Constellation Contains any of the current own neighbor
                            if (constellations[y].Contains(val))
                            {
                                hasConstellation = true;
                                //Iterate through every own neighbor
                                foreach ((int, int) neiPx in neighbors[x])
                                {
                                    //If the Constellation does not Contain the Neighbor already
                                    if (!(constellations[y].Contains(neiPx)))
                                    {
                                        //Add the Neigbor to the Constellation
                                        constellations[y].Add(neiPx);
                                    }
                                }
                            }
                        }
                    }
                    //If no Neighbor can be associated with a Constellation
                    if (hasConstellation == false)
                    {
                        //Start a new Constellation
                        constellations.Add(neighbors[x]);
                    }
                }
            }

            return constellations;

        }

        /// <summary>
        /// Returns a List of Coordinates of all Neighboring Black Pixels, with a threshold of 0.5
        /// </summary>
        /// <param name="image">The Image Double Representaion</param>
        /// <param name="imageWidth">Width of the Image</param>
        /// <param name="imageHeight">Height of the Image</param>
        /// <param name="pixel_X">The X Coordinate of the Pixel to be checked by Neighbors</param>
        /// <param name="pixel_Y">The Y Coordinate of the Pixel to be checked by Neighbors</param>
        /// <returns></returns>
        private static List<(int, int)> checkNeighbors (double[,] image, int imageWidth, int imageHeight, int pixel_X, int pixel_Y)
        {
            double threshold = 0.5;
            int minCheckX = -1;
            int maxCheckX = 1;
            int minCheckY = -1;
            int maxCheckY = 1;

            List<(int, int)> neighbors = new List<(int, int)>();

            //If The Pixel to be checked is not in the Corner of the Picture
            if (((pixel_X < imageWidth) && (pixel_X > 0)) && ((pixel_Y < imageHeight) && (pixel_Y > 0)))
            {  
            }
            //If the Pixel is on a Corner or Edge of the Picture
            else
            {
                if (pixel_X == imageWidth)
                {
                    maxCheckX = 0;
                }
                if (pixel_X == 0)
                {
                    minCheckX = 0;
                }
                if (pixel_Y == imageHeight)
                {
                    maxCheckY = 0;
                }
                if (pixel_Y == 0)
                {
                    minCheckY = 0;
                }
            }

            //Check for Neighbors
            for (int x = minCheckX; x <= maxCheckX; x++)
            {
                for (int y = minCheckY; y <= maxCheckY; y++)
                {
                    if (BWThreshold(image[pixel_X + x, pixel_Y + y], threshold) == 0)
                    {
                        neighbors.Add((pixel_X + x, pixel_Y + y));
                    }
                }
            }

            return neighbors;
            
            
        }

        /// <summary>
        /// Returns 1 if the Value is equal to or bigger than the Threshold
        /// </summary>
        /// <param name="value">The Value</param>
        /// <param name="threshold">The Threshold which the Value must be equal to or bigger, in order for the Function to return 1</param>
        /// <returns></returns>
        private static int BWThreshold(double value, double threshold)
        {
            return value < threshold ? 0 : 1;
        }

        /// <summary>
        /// Cleans the given image from Artifacts, based on the amount of pixels per constellation and the minimum Artifact Threshold
        /// </summary>
        /// <param name="image">The Image to be cleaned</param>
        /// <param name="imageWidth">The Width of the Image</param>
        /// <param name="imageHeight">The Height of the Image</param>
        /// <param name="minimumArtifactThreshold">The minimum amount of pixels a constellation must have in order to not count as an artifact</param>
        /// <returns></returns>
        public static (double[,], List<List<(int, int)>>) cleanup(double[,] image, int imageWidth, int imageHeight, int minimumArtifactThreshold)
        {
            List<List<(int, int)>> constellations = GetConstellations(image, imageWidth, imageHeight);

            List<List<(int, int)>> finalConstellations = new List<List<(int, int)>>();

            //Add only the Constellations who surpass the Minimum Artifact Threshold
            for (int x = 0; x < constellations.Count(); x++)
            {
                if (constellations[x].Count() >= minimumArtifactThreshold)
                {
                    finalConstellations.Add(constellations[x]);
                }
            }

            double[,] newImage = new double[imageWidth, imageHeight];

            //Set every Pixel in the double Image representation to 1
            for (int x = 0; x < imageWidth; x++)
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    newImage[x, y] = 1;
                }
            }

            //Set every Pixel from a Final Constellation to its original Pixel Value
            for (int x = 0; x < finalConstellations.Count(); x++)
            {
                for (int y = 0; y < finalConstellations[x].Count(); y++)
                {
                    newImage[finalConstellations[x][y].Item1, finalConstellations[x][y].Item2] = image[finalConstellations[x][y].Item1, finalConstellations[x][y].Item2];
                }
            }

            //Return the Cleaned Image and the Final Constellations
            return (newImage, finalConstellations);
        }
    }
}
