using Emgu.CV.Structure;
using Emgu.CV;
using System;
using Emgu.CV.CvEnum;
using System.Web.UI.WebControls;

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
        public static Image<Gray, byte> SobelFilter(Image<Gray, byte> inputImage)
        {
            inputImage = GaussianFilter(inputImage, 1.2);
            Image<Gray, byte> result = inputImage.Clone();


            for (int u = 1; u < inputImage.Rows - 1; u++)
            {
                for (int v = 1; v < inputImage.Cols - 1; v++)
                {
                    //masca pentru contururi orizontale
                    double Sx = inputImage[u + 1, v - 1].Intensity - inputImage[u - 1, v - 1].Intensity +
                                 2 * inputImage[u + 1, v].Intensity - 2 * inputImage[u - 1, v].Intensity +
                                 inputImage[u + 1, v + 1].Intensity - inputImage[u - 1, v + 1].Intensity;

                    //masca pentru contururi verticale
                    double Sy = inputImage[u - 1, v + 1].Intensity - inputImage[u - 1, v - 1].Intensity +
                                 2 * inputImage[u, v + 1].Intensity - 2 * inputImage[u, v - 1].Intensity +
                                 inputImage[u + 1, v + 1].Intensity - inputImage[u + 1, v - 1].Intensity;



                    double G = Math.Sqrt(Sx * Sx + Sy * Sy); //imagine gradient Grad(x,y)

                    if (G > 20)
                    {
                        result[u, v] = new Gray(G);
                    }
                    else
                    {
                        result[u, v] = new Gray(0);
                    }

                }
            }

            return result;
        }
        public static Image<Gray, byte> NonmaximaSuppression(Image<Gray, byte> inputImage)
        {
           inputImage = SobelFilter(inputImage);

            Image<Gray, byte> result = inputImage.Clone();

            for (int u = 1; u < inputImage.Rows - 1; u++)
            {
                for (int v = 1; v < inputImage.Cols - 1; v++)
                {
                    //masca pentru contururi orizontale
                    double Sx = inputImage[u + 1, v - 1].Intensity - inputImage[u - 1, v - 1].Intensity +
                                 2 * inputImage[u + 1, v].Intensity - 2 * inputImage[u - 1, v].Intensity +
                                 inputImage[u + 1, v + 1].Intensity - inputImage[u - 1, v + 1].Intensity;

                    //masca pentru contururi verticale
                    double Sy = inputImage[u - 1, v + 1].Intensity - inputImage[u - 1, v - 1].Intensity +
                                 2 * inputImage[u, v + 1].Intensity - 2 * inputImage[u, v - 1].Intensity +
                                 inputImage[u + 1, v + 1].Intensity - inputImage[u + 1, v - 1].Intensity;

                        double gradientAngle = Math.Atan2(Sy, Sx);

                        const double pi = Math.PI;

                        //reducem unghiul la intervalul[-pi / 2, pi / 2]
                        if (gradientAngle > pi / 2)
                        {
                            gradientAngle -= pi;
                        }
                        else if (gradientAngle <= -pi / 2)
                        {
                            gradientAngle += pi;
                        }

                        ThinEdges(result, u, v, gradientAngle);

                }
            }
            return result;
        }
        private static void ThinEdges(Image<Gray, byte> result, int u, int v, double angle)
        {
            const double pi = Math.PI;

            double currentPixelValue = result[u, v].Intensity;

            double neighborValue1 = 0, neighborValue2 = 0;

            if (angle >= -pi / 8 && angle <= pi / 8)
            {
                neighborValue1 = result[u - 1, v].Intensity;
                neighborValue2 = result[u + 1, v].Intensity;

                double maxGradient = Math.Max(Math.Max(currentPixelValue, neighborValue1), neighborValue2);

                if (maxGradient != currentPixelValue)
                    result[u, v] = new Gray(0);
                if (maxGradient != neighborValue1)
                    result[u - 1, v] = new Gray(0);
                if (maxGradient != neighborValue2)
                    result[u + 1, v] = new Gray(0);

                    if (currentPixelValue == neighborValue1 && currentPixelValue !=neighborValue2)
                    {
                        result[u-1, v] = new Gray(0); // Păstrăm doar pixelul la stânga
                    }
                    else if (currentPixelValue == neighborValue2 && currentPixelValue != neighborValue1)
                    {
                        result[u+1, v] = new Gray(0); // Păstrăm doar pixelul la dreapta
                    }
                
            }
            else if ((angle > -pi / 2 && angle <= -pi / 4) || (angle >= pi / 4 && angle <= pi / 2))
            {
                neighborValue1 = result[u, v - 1].Intensity;
                neighborValue2 = result[u, v + 1].Intensity;

                double maxGradient = Math.Max(Math.Max(currentPixelValue, neighborValue1), neighborValue2);
                if (maxGradient != currentPixelValue)
                    result[u, v] = new Gray(0);
                if (maxGradient != neighborValue1)
                    result[u, v - 1] = new Gray(0);
                if (maxGradient != neighborValue2)
                    result[u, v + 1] = new Gray(0);

                //cazul in care am doua valori egale
               
                   
                    if (currentPixelValue == neighborValue1 && currentPixelValue!=neighborValue2)
                    {
                        result[u, v - 1] = new Gray(0); // Păstrăm doar pixelul la stânga
                    }
                    else if (currentPixelValue == neighborValue2 && currentPixelValue != neighborValue1)
                    {
                        result[u, v + 1] = new Gray(0); // Păstrăm doar pixelul la dreapta
                    }
                
            }
            else if (angle > -3 * pi / 8 && angle < -pi / 8)
            {
                neighborValue1 = result[u - 1, v + 1].Intensity;
                neighborValue2 = result[u + 1, v - 1].Intensity;

                double maxGradient = Math.Max(Math.Max(currentPixelValue, neighborValue1), neighborValue2);

                if (maxGradient != currentPixelValue)
                    result[u, v] = new Gray(0);
                if (maxGradient != neighborValue1)
                    result[u - 1, v + 1] = new Gray(0);
                if (maxGradient != neighborValue2)
                    result[u + 1, v - 1] = new Gray(0);

                //cazul in care am doua valori egale
               
                    if (currentPixelValue == neighborValue1 && currentPixelValue != neighborValue2)
                    {
                        result[u-1, v + 1] = new Gray(0); // Păstrăm doar pixelul la stânga
                    }
                    else if (currentPixelValue == neighborValue2 && currentPixelValue != neighborValue1)
                    {
                        result[u + 1, v - 1] = new Gray(0); // Păstrăm doar pixelul la dreapta
                    }
                
            }
            else if (angle > pi / 8 && angle < 3 * pi / 8)
            {
                neighborValue1 = result[u - 1, v - 1].Intensity;
                neighborValue2 = result[u + 1, v + 1].Intensity;

                double maxGradient = Math.Max(Math.Max(currentPixelValue, neighborValue1), neighborValue2);
                if (maxGradient != currentPixelValue)
                    result[u, v] = new Gray(0);
                if (maxGradient != neighborValue1)
                    result[u - 1, v - 1] = new Gray(0);
                if (maxGradient != neighborValue2)
                    result[u + 1, v + 1] = new Gray(0);

                //cazul in care am doua valori egale
                
                    if (currentPixelValue == neighborValue1 && currentPixelValue != neighborValue2)
                    {
                        result[u - 1 , v - 1] = new Gray(0); // Păstrăm doar pixelul la stânga
                    }
                    else if (currentPixelValue == neighborValue2 && currentPixelValue != neighborValue1)
                    {
                        result[u +1 , v + 1] = new Gray(0); // Păstrăm doar pixelul la dreapta
                    }
                
            }
        }
        public static Image<Gray, byte> HysteresisThresholding(Image<Gray, byte> inputImage, double tMin, double tMax)
        {
            inputImage = NonmaximaSuppression(inputImage);

            Image<Gray, byte> result = inputImage.Clone();

            for (int u = 1; u < inputImage.Rows - 1; u++)
            {
                for (int v = 1; v < inputImage.Cols - 1; v++)
                {
                    double pixelValue = inputImage[u, v].Intensity;

                    if(pixelValue < tMin)
                    {
                        result[u, v] = new Gray(0);
                    }
                    else if(pixelValue > tMax)
                    {
                        result[u, v] = new Gray(255);
                    }
                    else

                    //cazul in care valoarea pixelului este intre tMin si tMax
                    //verific daca are vecini cu norma gradientului mai mare decat tMax
                    { 
                        bool hasStrongNeighbor = false;
                        for (int i = u - 1; i <= u + 1; i++)
                        {
                            for (int j = v - 1; j <= v + 1; j++)
                            {
                                if (result[i, j].Intensity > tMax)
                                {
                                    hasStrongNeighbor = true;
                                    break;
                                }
                            }
                            if (hasStrongNeighbor)
                                break;
                        }
                        if (hasStrongNeighbor)
                            result[u, v] = new Gray(255);
                        else
                            result[u, v] = new Gray(0);
                    }
                }
            }
            return result;
        }
        public static Image<Bgr, byte> ColorEdgesByOrientation(Image<Gray, byte> inputImage)
        {
            inputImage=GaussianFilter(inputImage,1.2);

            Image<Bgr, byte> result = inputImage.Clone().Convert<Bgr, byte>();


            for (int u = 1; u < inputImage.Rows - 1; u++)
            {
                for (int v = 1; v < inputImage.Cols - 1; v++)
                {
                    // Masca pentru contururi orizontale
                    double Sx = inputImage[u + 1, v - 1].Intensity - inputImage[u - 1, v - 1].Intensity +
                                 2 * inputImage[u + 1, v].Intensity - 2 * inputImage[u - 1, v].Intensity +
                                 inputImage[u + 1, v + 1].Intensity - inputImage[u - 1, v + 1].Intensity;

                    // Masca pentru contururi verticale
                    double Sy = inputImage[u - 1, v + 1].Intensity - inputImage[u - 1, v - 1].Intensity +
                                 2 * inputImage[u, v + 1].Intensity - 2 * inputImage[u, v - 1].Intensity +
                                 inputImage[u + 1, v + 1].Intensity - inputImage[u + 1, v - 1].Intensity;

                    double G = Math.Sqrt(Sx * Sx + Sy * Sy); //imagine gradient Grad(x,y)

                    if (G > 20)                    
                    {
                        double gradientAngle = Math.Atan2(Sy, Sx);

                        const double pi = Math.PI;

                        //reducem unghiul la intervalul[-pi / 2, pi / 2]
                        if (gradientAngle > pi / 2)
                        {
                            gradientAngle -= pi;
                        }
                        else if (gradientAngle <= -pi / 2)
                        {
                            gradientAngle += pi;
                        }

                        //Gray color2 = GetGrayImageByAngle(gradientAngle);

                        // Atribuim culoarea pixelului în funcție de categoria orientării
                        Bgr color = GetColorByOrientation(gradientAngle);

                        // Setăm culoarea pixelului în rezultat
                        result[u, v] = color;

                        //ThinEdges(result, u, v, gradientAngle);

                    }
                    else
                    {
                        result[u,v]= new Bgr(0,0,0);
                    }
                }
            }

            return result;
        }
        public static Image<Gray, byte> ColorEdgesGrayByOrientation(Image<Gray, byte> inputImage)
        {
           // inputImage = GaussianFilter(inputImage, 1.2);

            Image<Gray, byte> result = inputImage.Clone();


            for (int u = 1; u < inputImage.Rows - 1; u++)
            {
                for (int v = 1; v < inputImage.Cols - 1; v++)
                {
                    // Masca pentru contururi orizontale
                    double Sx = inputImage[u + 1, v - 1].Intensity - inputImage[u - 1, v - 1].Intensity +
                                 2 * inputImage[u + 1, v].Intensity - 2 * inputImage[u - 1, v].Intensity +
                                 inputImage[u + 1, v + 1].Intensity - inputImage[u - 1, v + 1].Intensity;

                    // Masca pentru contururi verticale
                    double Sy = inputImage[u - 1, v + 1].Intensity - inputImage[u - 1, v - 1].Intensity +
                                 2 * inputImage[u, v + 1].Intensity - 2 * inputImage[u, v - 1].Intensity +
                                 inputImage[u + 1, v + 1].Intensity - inputImage[u + 1, v - 1].Intensity;

                    double G = Math.Sqrt(Sx * Sx + Sy * Sy); //imagine gradient Grad(x,y)

                    if (G >100)
                    {
                        double gradientAngle = Math.Atan2(Sy, Sx);

                         gradientAngle = gradientAngle - Math.PI/2;

                       
                        const double pi = Math.PI;

                        //reducem unghiul la intervalul[-pi / 2, pi / 2]
                        if (gradientAngle > pi / 2)
                        {
                            gradientAngle -= pi;
                        }
                        else if (gradientAngle <= -pi / 2)
                        {
                            gradientAngle += pi;
                        }

                        Bgr color = GetColorByOrientation(gradientAngle);

                        ////reducem unghiul la intervalul[-pi, pi]
                        //if (gradientAngle < -Math.PI)
                        //{
                        //    gradientAngle += 2 * Math.PI;
                        //}
                        //else if (gradientAngle >= Math.PI)
                        //{
                        //    gradientAngle -= 2 * Math.PI;
                        //}

                        //Gray color2 = GetGrayImageByAngle(gradientAngle);

                        // Atribuim culoarea pixelului în funcție de categoria orientării
                       
                        // Setăm culoarea pixelului în rezultat
                        //result[u, v] = color2;

                        //ApplyThinEdges(result, u, v, gradientAngle);

                    }
                    else
                    {
                        result[u, v] = new Gray(0);
                    }
                }
            }

            return result;
        }
        private static Gray GetGrayImageByAngle(double angle)
        {
            double angleDegrees = angle * 180 / Math.PI;
         
            double grayColor = (angleDegrees + 180) * 127 / 360 + 128; 
            return new Gray(grayColor);
        }
        private static Bgr GetColorByOrientation(double angle)
        {

            const double pi = Math.PI;

            // Atribuim culoarea în funcție de orientare
            if (angle >= -pi / 8 && angle <= pi / 8)
            {
                return new Bgr(0, 0, 255); // Roșu (orizontal)
            }
            else if ((angle > -pi / 2 && angle <= - pi / 4) || (angle >=  pi /4 && angle <= pi / 2))
            {
                return new Bgr(0, 255, 0); // Verde (vertical)
            }
            else if (angle > pi / 8 && angle < 3 * pi / 8)
            {
                return new Bgr(255, 0, 0); // Albastru (diagonală 2)
            }
            else if (angle > -3 * pi / 8 && angle < -pi / 8)
            {
                return new Bgr(0, 255, 255); // Galben (diagonală 1)
            }
            else
            {
                return new Bgr(0, 0, 0); // Negru pentru orice altceva
            }
        }






    }
}