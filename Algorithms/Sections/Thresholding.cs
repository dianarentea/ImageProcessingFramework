using Emgu.CV.Structure;
using Emgu.CV;
using System;

namespace Algorithms.Sections
{
    public class Thresholding
    {
        #region Thresholding
        public static Image<Gray, byte> ThresholdingMethod(Image<Gray, byte> inputImage,
        byte threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (inputImage.Data[y, x, 0] >= threshold)
                    {
                        result.Data[y, x, 0] = 255;
                    }
                    else // redundant, constructorul clasei Image coloreaza toti pixelii cu negru
                    {
                        result.Data[y, x, 0] = 0;
                    }
                }
            }
            return result;
        }
        public static Image<Gray, byte> twoThresholding(Image<Gray, byte> inputImage,
       int t1, int t2)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (inputImage.Data[y, x, 0] >= 0 && inputImage.Data[y, x, 0] <= t1)
                    {
                        result.Data[y, x, 0] = 0;
                    }
                    else if (inputImage.Data[y, x, 0] > t1 && inputImage.Data[y, x, 0] <= t2)
                    {
                        result.Data[y, x, 0] = 128;
                    }
                    else if (inputImage.Data[y, x, 0] > t2 && inputImage.Data[y, x, 0] <= 255)
                    {
                        result.Data[y, x, 0] = 255;
                    }
                }
            }
            return result;
        }

        public static Tuple<int, int> OtsuDoubleThreshold(double[] relativeHistogram)
        {

            int t1 = 0;
            int t2 = 1;
            double sigma_max = 0;

            double u = 0;
            for (int k = 0; k < 256; k++)
            {
                u += k * relativeHistogram[k];
            }


            for (int k1 = 0; k1 < 254; k1++)
            {
                for (int k2 = k1 + 1; k2 < 255; k2++)
                {
                    double p1 = 0, p2 = 0, p3 = 0, u1 = 0, u2 = 0, u3 = 0;

                    for (int k = 0; k <= k1; k++)
                    {
                        p1 += relativeHistogram[k];
                        u1 += k * relativeHistogram[k];
                    }
                    if (p1 != 0)
                    {
                        u1 /= p1;
                    }

                    for (int k = k1 + 1; k <= k2; k++)
                    {
                        p2 += relativeHistogram[k];
                        u2 += k * relativeHistogram[k];
                    }
                    if (p2 != 0)
                    {
                        u2 /= p2;
                    }

                    for (int k = k2 + 1; k < 256; k++)
                    {
                        p3 += relativeHistogram[k];
                        u3 += k * relativeHistogram[k];
                    }
                    if (p3 != 0)
                    {
                        u3 /= p3;
                    }

                    double sigma = p1 * (u1 - u) * (u1 - u) + p2 * (u2 - u) * (u2 - u) + p3 * (u3 - u) * (u3 - u);
                    if (sigma > sigma_max)
                    {
                        sigma_max = sigma;
                        t1 = k1;
                        t2 = k2;
                    }
                }
            }
            return new Tuple<int, int>(t1, t2);
        }
        #endregion
    }
}