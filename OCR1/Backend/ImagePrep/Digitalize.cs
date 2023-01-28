using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCR1.Backend.ImagePrep;
using System.Drawing;

namespace OCR1.Backend.ImagePrep
{
    class Digitalize
    {
        /// <summary>
        /// Return a Black and White Double. Value can be between 0 and 1.
        /// </summary>
        /// <param name="imageR">The Red Value of the Pixel</param>
        /// <param name="imageG">The Green Value of the Pixel</param>
        /// <param name="imageB">The Blue Value of the Pixel</param>
        /// <param name="imageA">The Pixel Alpha, by which the Value gets multiplied with in the End</param>
        /// <returns></returns>
        private static double GetBWDouble(int imageR, int imageG, int imageB, int imageA)
        {
            return (((imageR + imageG + imageB) / 3) / 255) * (imageA / 255);
        }

        /// <summary>
        /// Returns a Black and White Integer. Value can be 0 or 1.
        /// </summary>
        /// <param name="imageR">The Red Value of the Pixel</param>
        /// <param name="imageG">The Green Value of the Pixel</param>
        /// <param name="imageB">The Red Value of the Pixel</param>
        /// <param name="imageA">The Pixel Alpha, by which the Value gets multiplied with in the End</param>
        /// <returns></returns>
        private static int GetBWInt(int imageR, int imageG, int imageB, int imageA)
        {
            return ((((imageR + imageG + imageB) / 3) / 255) * (imageA / 255)) <= 128 ? 0:1;
        }

        /// <summary>
        /// Returns an Array Representation of the image, where every Pixel is represented by a Black and White Value (double)
        /// </summary>
        /// <param name="image">The Image to be Digitalized</param>
        /// <returns></returns>
        public static double[,] digitalize(Bitmap image)
        {
            double[,] img = new double[image.Width, image.Height];
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    img[x, y] = GetBWDouble(image.GetPixel(x, y).R, image.GetPixel(x, y).G, image.GetPixel(x, y).B, image.GetPixel(x, y).A);
                }
            }
            return img;
        }

    }
}
