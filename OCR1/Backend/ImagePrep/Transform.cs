using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OCR1.Backend.ImagePrep;

namespace OCR1.Backend.ImagePrep
{
    class Transform
    {
        /// <summary>
        /// Returns a Black and White Image from the given image array
        /// </summary>
        /// <param name="image">The Black and White Double Array Representation of an Image</param>
        /// <param name="height">The Image's Height</param>
        /// <param name="width">The Image's Width</param>
        /// <returns></returns>
        public static Bitmap imageFromArray(double[,] image, int height, int width)
        {
            Bitmap img = new Bitmap(width, height);

            //Set every Pixel in the Image to the BW double from the Array mutiplied with 255
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    img.SetPixel(x, y, Color.FromArgb(255, Convert.ToInt16(image[x, y] * 255), Convert.ToInt16(image[x, y] * 255), Convert.ToInt16(image[x, y] * 255)));
                }
            }

            return img;
        }
    }
}
