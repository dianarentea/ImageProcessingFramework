using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {
        public static Image<Bgr, byte> Erosion(Image<Bgr, byte> inputImage, int kernelSize)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            int kernelRadius = kernelSize / 2;

            for (int i = kernelRadius; i < inputImage.Height - kernelRadius; i++)
            {
                for (int j = kernelRadius; j < inputImage.Width - kernelRadius; j++)
                {
                    Bgr minColor = new Bgr(100, 100, 100); // Inițializare cu alb (maximul pentru fiecare canal)

                    for (int k = -kernelRadius; k <= kernelRadius; k++)
                    {
                        for (int l = -kernelRadius; l <= kernelRadius; l++)
                        {
                            Bgr currentColor = inputImage[i + k, j + l];

                          //caut pixelul minim din vecintatea definita de kernel
                            if (currentColor.Blue < minColor.Blue ||
                                (currentColor.Blue == minColor.Blue && currentColor.Green < minColor.Green) ||
                                (currentColor.Blue == minColor.Blue && currentColor.Green == minColor.Green && currentColor.Red < minColor.Red))
                            {
                                minColor = currentColor;
                            }

                        }
                    }

                    result[i, j] = minColor;
                }
            }

            return result;
        }

        public static Image<Bgr, byte> Dilation(Image<Bgr, byte> inputImage, int kernelSize)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            int kernelRadius = kernelSize / 2;

            for (int i = kernelRadius; i < inputImage.Height - kernelRadius; i++)
            {
                for (int j = kernelRadius; j < inputImage.Width - kernelRadius; j++)
                {
                    Bgr maxColor = new Bgr(0, 0, 0); // Inițializare cu negru (minimul pentru fiecare canal)

                    for (int k = -kernelRadius; k <= kernelRadius; k++)
                    {
                        for (int l = -kernelRadius; l <= kernelRadius; l++)
                        {
                            Bgr currentColor = inputImage[i + k, j + l];

                         //caut pixelul maxim din vecintatea definita de kernel
                            if (currentColor.Blue > maxColor.Blue ||
                                (currentColor.Blue == maxColor.Blue && currentColor.Green > maxColor.Green) ||
                                (currentColor.Blue == maxColor.Blue && currentColor.Green == maxColor.Green && currentColor.Red > maxColor.Red))
                            {
                                maxColor = currentColor;
                            }
                        }
                    }

                    result[i, j] = maxColor;
                }
            }

            return result;
        }


        //opnening using algorithm on color image but using pixel order in raport with the origin for calculatin the min and max in the neighborhood defined by the kernel

        public static Image<Bgr, byte> Opening(Image<Bgr, byte> inputImage, int kernelSize)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            Image<Bgr, byte> erodateImage = Erosion(result, kernelSize);
            Image<Bgr, byte> dilatedImage = Dilation(erodateImage, kernelSize);
            return dilatedImage;
        }



    }
}