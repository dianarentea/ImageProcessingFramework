using Emgu.CV.Structure;
using Emgu.CV;
using System;

namespace Algorithms.Sections
{
   
    public class Filters
    {
        #region bilateral filter
        public static Image<Gray, byte> BilateralFilterGray(Image<Gray, byte> inputImage, double sigmaD, double sigmaR)
        {
            int D = (int)(3.5 * sigmaD);

            if (D % 2 == 0)
            {
                D++;
            }

            Image<Gray, byte> result = inputImage.Clone();



            for (int u = D; u < inputImage.Rows - D; u++)
            {
                for (int v = D; v < inputImage.Cols - D; v++)
                {
                    double S = 0;  // Sum of weighted pixel values
                    double W = 0;  // Sum of weights
                    double a = inputImage[u, v].Intensity;  // Center pixel value

                    for (int m = -D; m <= D; m++)
                    {
                        for (int n = -D; n <= D; n++)
                        {
                            double b = inputImage[u + m, v + n].Intensity;  // Off-center pixel value
                            double wd = Math.Exp(-((m * m + n * n) / (2.0 * sigmaD * sigmaD)));  // Domain coefficient
                            double wr = Math.Exp(-((a - b) * (a - b) / (2.0 * sigmaR * sigmaR)));  // Range coefficient
                            double w = wd * wr;  // Composite coefficient
                            S += w * b;
                            W += w;
                        }
                    }

                    result[u, v] = new Gray(S / W);
                }
            }

            return result;
        }

        public static Image<Bgr, byte> BilateralFilterColor(Image<Bgr, byte> inputImage, double sigmaD, double sigmaR)
        {
            int M = inputImage.Rows;
            int N = inputImage.Cols;

            int D = (int)(3.5 * sigmaD);

            if (D % 2 == 0)
            {
                D++;
            }
            Image<Bgr, byte> filteredImage = inputImage.Clone();

            for (int u = D; u < M - D; u++)
            {
                for (int v = D; v < N - D; v++)
                {
                    MCvScalar S = new MCvScalar(0, 0, 0);  // Sum of weighted pixel vectors
                    double W = 0;  // Sum of pixel weights (scalar)
                    MCvScalar a = inputImage[u, v].MCvScalar;  // Center pixel vector

                    for (int m = -D; m <= D; m++)
                    {
                        for (int n = -D; n <= D; n++)
                        {
                            MCvScalar b = inputImage[u + m, v + n].MCvScalar;  // Off-center pixel vector
                            double wd = Math.Exp(-((m * m + n * n) / (2.0 * sigmaD * sigmaD)));  // Domain coefficient

                            // Calculate color distance (Euclidean distance)
                            double colorDistance = Math.Sqrt(Math.Pow(a.V0 - b.V0, 2) + Math.Pow(a.V1 - b.V1, 2) + Math.Pow(a.V2 - b.V2, 2));
                            double wr = Math.Exp(-(colorDistance * colorDistance) / (2.0 * sigmaR * sigmaR));  // Range coefficient

                            double w = wd * wr;  // Composite coefficient
                            S.V0 += w * b.V0;
                            S.V1 += w * b.V1;
                            S.V2 += w * b.V2;
                            W += w;

                        }
                    }

                    filteredImage[u, v] = new Bgr(S.V0 / W, S.V1 / W, S.V2 / W);
                }
            }

            return filteredImage;
        }
        #endregion
        public static Image<Gray, byte> GaussianFilter(Image<Gray, byte> inputImage, double sigma)
        {
            int kernelSize = (int)(4 * sigma);
            if(kernelSize % 2 == 0)
            {
                kernelSize++;
            }
            int halfKernel = kernelSize / 2;

            Image<Gray, byte> result = inputImage.Clone();

            for (int u = halfKernel; u < inputImage.Rows - halfKernel; u++)
            {
                for (int v = halfKernel; v < inputImage.Cols - halfKernel; v++)
                {
                    double S = 0; // Suma valorilor pixelilor ponderată
                    double W = 0; // Suma ponderilor

                    for (int m = -halfKernel; m <= halfKernel; m++)
                    {
                        for (int n = -halfKernel; n <= halfKernel; n++)
                        {
                            double b = inputImage[u + m, v + n].Intensity; // Valoarea pixelului din vecinătate
                            double wr = Math.Exp(-((m * m + n * n) / (2.0 * sigma * sigma))); // Coeficientul Gaussian în domeniul valorilor pixelilor
                            double w = wr; // Ponderarea Gaussiană
                            S += w * b;
                            W += w;
                        }
                    }

                    result[u, v] = new Gray(S / W);
                }
            }

            return result;
        }


    }
}