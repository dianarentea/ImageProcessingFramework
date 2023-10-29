using Algorithms.Utilities;
using Castle.Core;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.UI;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Mirror

        public static Image<Gray, byte> Mirror(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = Copy(inputImage);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width/2; x++)
                {
                    Utils.Swap(ref result.Data[y, x, 0], ref result.Data[y, inputImage.Width-1-x, 0]);
                    
                    
                }
            }
            return result;
        }
        

        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = Copy(inputImage);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width / 2; x++)
                {
                    Utils.Swap(ref result.Data[y, x, 0], ref result.Data[y, inputImage.Width - 1 - x, 0]);
                    Utils.Swap(ref result.Data[y, x, 1], ref result.Data[y, inputImage.Width - 1 - x, 1]);
                    Utils.Swap(ref result.Data[y, x, 2], ref result.Data[y, inputImage.Width - 1 - x, 2]);

                }
            }
            return result;
        }
        #endregion

        #region RotateClockwise
        public static Image<Gray, byte> RotateClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = Copy(inputImage);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[inputImage.Size.Width - 1 - x, y, 0];

                }
            }
            return result;
        }
        public static Image<Bgr, byte> RotateClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = Copy(inputImage);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[inputImage.Size.Width - 1 - x, y, 0];
                    result.Data[y, x, 1] = inputImage.Data[inputImage.Size.Width - 1 - x, y, 1];
                    result.Data[y, x, 2] = inputImage.Data[inputImage.Size.Width - 1 - x, y, 2];

                }
            }
            return result;
        }
        #endregion

        #region RotateAntiClockwise
        public static Image<Gray, byte> RotateAntiClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = Copy(inputImage);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[x, inputImage.Size.Height - 1 - y, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateAntiClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = Copy(inputImage);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = inputImage.Data[x, inputImage.Size.Height - 1 - y, 0];
                    result.Data[y, x, 1] = inputImage.Data[x, inputImage.Size.Height - 1 - y, 1];
                    result.Data[y, x, 2] = inputImage.Data[x, inputImage.Size.Height - 1 - y, 2];
                }
            }
            return result;
        }
        #endregion

        #region Crop Image
        public static Image<Gray, byte> CropImage(Image<Gray, byte> inputImage, int x, int y, int width, int height)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width ; j++)
                {
                    result.Data[i, j, 0] = inputImage.Data[i + y, j + x, 0];
                }
            }
            return result;
        }   

        public static Image<Bgr, byte> CropImage(Image<Bgr, byte> inputImage, int x, int y, int width, int height)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width ; j++)
                {
                    result.Data[i, j, 0] = inputImage.Data[i + y, j + x, 0];
                    result.Data[i, j, 1] = inputImage.Data[i + y, j + x, 1];
                    result.Data[i, j, 2] = inputImage.Data[i + y, j + x, 2];
                }
            }
            return result;
        }

        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        #endregion

        #region Thresholding
        public static Image<Gray, byte> Thresholding(Image<Gray, byte> inputImage,
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
                    if (inputImage.Data[y, x, 0] >=0 && inputImage.Data[y,x,0]<=t1)
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
            //verificare sa nu existe nuante de gri care sa nu apa deloc (relativeHistogram[i]!=0 )
        

            int t1 = 0;
            int t2 = 1;
            double sigma_max = 0;

            double u = 0;
            for (int k = 0; k < 256; k++)
            {
                u += k * relativeHistogram[k];
            }


            for (int k1=0; k1<254; k1++)
            {
                for(int k2=k1+1; k2<255; k2++)
                {
                double p1=0,p2=0,p3 = 0,u1 = 0,u2 = 0,u3 = 0;

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

        #region Contrast
        public static int[] CalculateLUT(double E, double m)
        {
            int[] lut = new int[256];

            // c pentru a satisface condiția Tn3(255) = 255
            double c = (1 - Math.Pow(255, E) /(Math.Pow(255,E)+Math.Pow(m,E)))*(1/255);

            for (int r = 0; r < 256; r++)
            {
                double transformedValue = 255 * (Math.Pow(r, E) / (Math.Pow(r, E) + Math.Pow(m, E)) + c * r);
                lut[r] = (int)Math.Round(transformedValue);
            }

            return lut;
        }

        public static Image<Gray, byte> ApplyLUT(Image<Gray, byte> inputImage, int[] lut)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)lut[inputImage.Data[y, x, 0]];

                }
            }

            return result;
        }

        public static Image<Bgr, byte> ApplyLUT(Image<Bgr, byte> inputImage, int[] lut)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)lut[inputImage.Data[y, x, 0]];
                    result.Data[y, x, 1] = (byte)lut[inputImage.Data[y, x, 1]];
                    result.Data[y, x, 2] = (byte)lut[inputImage.Data[y, x, 2]];
                }
            }

            return result;
        }



        #endregion
    }
}