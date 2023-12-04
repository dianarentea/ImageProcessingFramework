using Emgu.CV;
using Emgu.CV.Structure;

using System.Windows;
using System.Drawing.Imaging;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using Framework.View;
using static Framework.Utilities.DataProvider;
using static Framework.Utilities.FileHelper;
using static Framework.Utilities.DrawingHelper;
using static Framework.Converters.ImageConverter;

using Algorithms.Sections;
using Algorithms.Tools;
using Algorithms.Utilities;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace Framework.ViewModel
{
    public class MenuCommands : BaseVM
    {
        private readonly MainVM _mainVM;

        public MenuCommands(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        private ImageSource InitialImage
        {
            get => _mainVM.InitialImage;
            set => _mainVM.InitialImage = value;
        }

        private ImageSource ProcessedImage
        {
            get => _mainVM.ProcessedImage;
            set => _mainVM.ProcessedImage = value;
        }

        private double ScaleValue
        {
            get => _mainVM.ScaleValue;
            set => _mainVM.ScaleValue = value;
        }

        #region File

        #region Load grayscale image
        private RelayCommand _loadGrayImageCommand;
        public RelayCommand LoadGrayImageCommand
        {
            get
            {
                if (_loadGrayImageCommand == null)
                    _loadGrayImageCommand = new RelayCommand(LoadGrayImage);
                return _loadGrayImageCommand;
            }
        }

        private void LoadGrayImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a gray picture");
            if (fileName != null)
            {
                GrayInitialImage = new Image<Gray, byte>(fileName);
                InitialImage = Convert(GrayInitialImage);
            }
        }
        #endregion

        #region Load color image
        private ICommand _loadColorImageCommand;
        public ICommand LoadColorImageCommand
        {
            get
            {
                if (_loadColorImageCommand == null)
                    _loadColorImageCommand = new RelayCommand(LoadColorImage);
                return _loadColorImageCommand;
            }
        }

        private void LoadColorImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a color picture");
            if (fileName != null)
            {
                ColorInitialImage = new Image<Bgr, byte>(fileName);
                InitialImage = Convert(ColorInitialImage);
            }
        }
        #endregion

        #region Save processed image
        private ICommand _saveProcessedImageCommand;
        public ICommand SaveProcessedImageCommand
        {
            get
            {
                if (_saveProcessedImageCommand == null)
                    _saveProcessedImageCommand = new RelayCommand(SaveProcessedImage);
                return _saveProcessedImageCommand;
            }
        }

        private void SaveProcessedImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, " +
                    "please load and process an image first!");
                return;
            }

            string imagePath = SaveFileDialog("image.jpg");
            if (imagePath != null)
            {
                GrayProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                ColorProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                OpenImage(imagePath);
            }
        }
        #endregion

        #region Save both images
        private ICommand _saveImagesCommand;
        public ICommand SaveImagesCommand
        {
            get
            {
                if (_saveImagesCommand == null)
                    _saveImagesCommand = new RelayCommand(SaveImages);
                return _saveImagesCommand;
            }
        }

        private void SaveImages(object parameter)
        {
            if (GrayInitialImage == null && ColorInitialImage == null)
            {
                MessageBox.Show("If you want to save both images, " +
                    "please load and process an image first!");
                return;
            }

            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save both images, " +
                    "please process your image first!");
                return;
            }

            string imagePath = SaveFileDialog("image.jpg");
            if (imagePath != null)
            {
                IImage processedImage = null;
                if (GrayInitialImage != null && GrayProcessedImage != null)
                    processedImage = Utils.Combine(GrayInitialImage, GrayProcessedImage);

                if (GrayInitialImage != null && ColorProcessedImage != null)
                    processedImage = Utils.Combine(GrayInitialImage, ColorProcessedImage);

                if (ColorInitialImage != null && GrayProcessedImage != null)
                    processedImage = Utils.Combine(ColorInitialImage, GrayProcessedImage);

                if (ColorInitialImage != null && ColorProcessedImage != null)
                    processedImage = Utils.Combine(ColorInitialImage, ColorProcessedImage);

                processedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                OpenImage(imagePath);
            }
        }
        #endregion

        #region Exit
        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(Exit);
                return _exitCommand;
            }
        }

        private void Exit(object parameter)
        {
            CloseWindows();
            System.Environment.Exit(0);
        }
        #endregion

        #endregion

        #region Edit

        #region Remove drawn shapes from initial canvas
        private ICommand _removeInitialDrawnShapesCommand;
        public ICommand RemoveInitialDrawnShapesCommand
        {
            get
            {
                if (_removeInitialDrawnShapesCommand == null)
                    _removeInitialDrawnShapesCommand = new RelayCommand(RemoveInitialDrawnShapes);
                return _removeInitialDrawnShapesCommand;
            }
        }

        private void RemoveInitialDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from processed canvas
        private ICommand _removeProcessedDrawnShapesCommand;
        public ICommand RemoveProcessedDrawnShapesCommand
        {
            get
            {
                if (_removeProcessedDrawnShapesCommand == null)
                    _removeProcessedDrawnShapesCommand = new RelayCommand(RemoveProcessedDrawnShapes);
                return _removeProcessedDrawnShapesCommand;
            }
        }

        private void RemoveProcessedDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from both canvases
        private ICommand _removeDrawnShapesCommand;
        public ICommand RemoveDrawnShapesCommand
        {
            get
            {
                if (_removeDrawnShapesCommand == null)
                    _removeDrawnShapesCommand = new RelayCommand(RemoveDrawnShapes);
                return _removeDrawnShapesCommand;
            }
        }

        private void RemoveDrawnShapes(object parameter)
        {
            var canvases = (object[])parameter;
            RemoveUiElements(canvases[0] as Canvas);
            RemoveUiElements(canvases[1] as Canvas);
        }
        #endregion

        #region Clear initial canvas
        private ICommand _clearInitialCanvasCommand;
        public ICommand ClearInitialCanvasCommand
        {
            get
            {
                if (_clearInitialCanvasCommand == null)
                    _clearInitialCanvasCommand = new RelayCommand(ClearInitialCanvas);
                return _clearInitialCanvasCommand;
            }
        }

        private void ClearInitialCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayInitialImage = null;
            ColorInitialImage = null;
            InitialImage = null;
        }
        #endregion

        #region Clear processed canvas
        private ICommand _clearProcessedCanvasCommand;
        public ICommand ClearProcessedCanvasCommand
        {
            get
            {
                if (_clearProcessedCanvasCommand == null)
                    _clearProcessedCanvasCommand = new RelayCommand(ClearProcessedCanvas);
                return _clearProcessedCanvasCommand;
            }
        }

        private void ClearProcessedCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayProcessedImage = null;
            ColorProcessedImage = null;
            ProcessedImage = null;
        }
        #endregion

        #region Closing all open windows and clear both canvases
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                    _clearCommand = new RelayCommand(Clear);
                return _clearCommand;
            }
        }

        private void Clear(object parameter)
        {
            CloseWindows();

            ScaleValue = 1;

            var canvases = (object[])parameter;
            ClearInitialCanvas(canvases[0] as Canvas);
            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion

        #endregion

        #region Tools

        #region Magnifier
        private ICommand _magnifierCommand;
        public ICommand MagnifierCommand
        {
            get
            {
                if (_magnifierCommand == null)
                    _magnifierCommand = new RelayCommand(Magnifier);
                return _magnifierCommand;
            }
        }

        private void Magnifier(object parameter)
        {
            if (MagnifierOn == true) return;
            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            MagnifierWindow magnifierWindow = new MagnifierWindow();
            magnifierWindow.Show();
        }
        #endregion

        #region Display Gray/Color levels

        #region On row
        private ICommand _displayLevelsOnRowCommand;
        public ICommand DisplayLevelsOnRowCommand
        {
            get
            {
                if (_displayLevelsOnRowCommand == null)
                    _displayLevelsOnRowCommand = new RelayCommand(DisplayLevelsOnRow);
                return _displayLevelsOnRowCommand;
            }
        }

        private void DisplayLevelsOnRow(object parameter)
        {
            if (RowColorLevelsOn == true) return;
            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Row);
            window.Show();
        }
        #endregion

        #region On column
        private ICommand _displayLevelsOnColumnCommand;
        public ICommand DisplayLevelsOnColumnCommand
        {
            get
            {
                if (_displayLevelsOnColumnCommand == null)
                    _displayLevelsOnColumnCommand = new RelayCommand(DisplayLevelsOnColumn);
                return _displayLevelsOnColumnCommand;
            }
        }

        private void DisplayLevelsOnColumn(object parameter)
        {
            if (ColumnColorLevelsOn == true) return;
            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Column);
            window.Show();
        }
        #endregion

        #endregion

        #region Visualize image histogram

        #region Initial image histogram
        private ICommand _histogramInitialImageCommand;
        public ICommand HistogramInitialImageCommand
        {
            get
            {
                if (_histogramInitialImageCommand == null)
                    _histogramInitialImageCommand = new RelayCommand(HistogramInitialImage);
                return _histogramInitialImageCommand;
            }
        }

        private void HistogramInitialImage(object parameter)
        {
            if (InitialHistogramOn == true) return;
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            HistogramWindow window = null;

            if (ColorInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialColor);
            }
            else if (GrayInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialGray);
            }

            window.Show();
        }
        #endregion

        #region Processed image histogram
        private ICommand _histogramProcessedImageCommand;
        public ICommand HistogramProcessedImageCommand
        {
            get
            {
                if (_histogramProcessedImageCommand == null)
                    _histogramProcessedImageCommand = new RelayCommand(HistogramProcessedImage);
                return _histogramProcessedImageCommand;
            }
        }

        private void HistogramProcessedImage(object parameter)
        {
            if (ProcessedHistogramOn == true) return;
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image !");
                return;
            }

            HistogramWindow window = null;

            if (ColorProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedColor);
            }
            else if (GrayProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedGray);
            }

            window.Show();
        }
        #endregion

        #endregion


        #region Copy image
        private ICommand _copyImageCommand;
        public ICommand CopyImageCommand
        {
            get
            {
                if (_copyImageCommand == null)
                    _copyImageCommand = new RelayCommand(CopyImage);
                return _copyImageCommand;
            }
        }

        private void CopyImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Copy(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Copy(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
        }
        #endregion

        #region Invert image
        private ICommand _invertImageCommand;
        public ICommand InvertImageCommand
        {
            get
            {
                if (_invertImageCommand == null)
                    _invertImageCommand = new RelayCommand(InvertImage);
                return _invertImageCommand;
            }
        }

        private void InvertImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Invert(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Invert(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Mirror image
        private ICommand _mirrorImageCommand;
        public ICommand  MirrorImageCommand
        {
            get
            {
                if (_mirrorImageCommand == null)
                    _mirrorImageCommand = new RelayCommand(MirrorImage);
                return _mirrorImageCommand;
            }
        }
        private void MirrorImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Mirror(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Mirror(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Rotate clockwise image

        private ICommand _rotateClockwiseImageCommand;
        public ICommand RotateClockwiseImageCommand
        {
            get
            {
                if (_rotateClockwiseImageCommand == null)
                    _rotateClockwiseImageCommand = new RelayCommand(RotateClockwiseImage);
                return _rotateClockwiseImageCommand;
            }
        }

        private void RotateClockwiseImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateClockwise(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateClockwise(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }


        #endregion

        #region Rotate Anticlockwise image

        private ICommand _rotateAnticlockwiseImageCommand;

        public ICommand RotateAnticlockwiseImageCommand
        {
            get
            {
                if (_rotateAnticlockwiseImageCommand == null)
                    _rotateAnticlockwiseImageCommand = new RelayCommand(RotateAnticlockwiseImage);
                return _rotateAnticlockwiseImageCommand;
            }
        }
        private void RotateAnticlockwiseImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateAntiClockwise(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateAntiClockwise(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }

        #endregion

        #region Crop Image
        private ICommand _cropImageCommand;
        public ICommand CropImageCommand
        {
            get
            {
                if (_cropImageCommand == null)
                    _cropImageCommand = new RelayCommand(CropImage);
                return _cropImageCommand;
            }
        }
        private void CropImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            if (VectorOfMousePosition.Count > 1)
            {
                int pos = VectorOfMousePosition.Count - 1;
                Point firstClick = VectorOfMousePosition[pos];
                Point secondClick = VectorOfMousePosition[pos - 1];

                double minWidth = Math.Min(firstClick.Y, secondClick.Y);
                double maxWidth = Math.Max(firstClick.Y, secondClick.Y);
                double minHeight = Math.Min(firstClick.X, secondClick.X);
                double maxHeight = Math.Max(firstClick.X, secondClick.X);

                Point start = new Point(minHeight, minWidth);
                Point end = new Point(maxHeight, maxWidth);

                if (parameter is object[] parameters && parameters.Length == 2)
                {

                    Canvas initialCanvas = parameters[0] as Canvas;
                    Canvas processedCanvas = parameters[1] as Canvas;

                    ClearProcessedCanvas(processedCanvas);
                    ClearProcessedCanvas(initialCanvas);

                    DrawRectangle(initialCanvas, start, end, 5, Brushes.Red, ScaleValue);

                    if (GrayInitialImage != null)
                    {
                        GrayProcessedImage = Tools.CropImage(GrayInitialImage, (int)start.X, (int)start.Y, (int)end.X, (int)end.Y);
                        ProcessedImage = Convert(GrayProcessedImage);
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Tools.CropImage(ColorInitialImage, (int)start.X, (int)start.Y, (int)end.X, (int)end.Y);
                        ProcessedImage = Convert(ColorProcessedImage);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select at least two points");
            }
        }
        #endregion

        #region Convert color image to grayscale image
        private ICommand _convertImageToGrayscaleCommand;
        public ICommand ConvertImageToGrayscaleCommand
        {
            get
            {
                if (_convertImageToGrayscaleCommand == null)
                    _convertImageToGrayscaleCommand = new RelayCommand(ConvertImageToGrayscale);
                return _convertImageToGrayscaleCommand;
            }
        }

        private void ConvertImageToGrayscale(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("It is possible to convert only color images !");
            }
        }
        #endregion

        #endregion

        #region Pointwise operations

        #region Contrast

        private ICommand _ContrastCommand;
        public ICommand ContrastCommand
        {
            get
            {
                if (_ContrastCommand == null)
                    _ContrastCommand = new RelayCommand(ContrastChange);
                return _ContrastCommand;
            }
        }

        private void ContrastChange(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);

            List<string> parameters = new List<string>();
            parameters.Add("Add E value: ");
            parameters.Add("Add m value:");
            DialogBox box = new DialogBox(_mainVM, parameters);
            box.ShowDialog();
            List<double> values = box.GetValues();


            if (values!=null)
            {
                if (GrayInitialImage != null)
                {
                    int[] lut = Tools.CalculateLUT(values[0], values[1]);
                    GrayProcessedImage = Tools.ApplyLUT(GrayInitialImage, lut);
                    ProcessedImage = Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    int[] lut = Tools.CalculateLUT(values[0], values[1]);
                    ColorProcessedImage = Tools.ApplyLUT(ColorInitialImage, lut);
                    ProcessedImage = Convert(ColorProcessedImage);
                }
            }
        }

        private ICommand _brightnessCommand;
        public ICommand BrightnessCommand
        {
            get
            {
                if (_brightnessCommand == null)
                    _brightnessCommand = new RelayCommand(Brightness);
                return _brightnessCommand;
            }
        }
        private void Brightness
            (object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
         
            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                int[] lut = Tools.CalculateLUT(2.5, 50);
                GrayProcessedImage = Tools.ApplyLUT(GrayInitialImage, lut);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                int[] lut = Tools.CalculateLUT(2.5, 50);
                ColorProcessedImage = Tools.ApplyLUT(ColorInitialImage, lut);
                ProcessedImage = Convert(ColorProcessedImage);
            }

        }


        #endregion
        #endregion

        #region Thresholding
        private ICommand _thresholdingCommand;
        public ICommand ThresholdingCommand
        {
            get
            {
                if (_thresholdingCommand == null)
                    _thresholdingCommand = new RelayCommand(ThresholdingImage);
                return _thresholdingCommand;
            }
        }
        private void ThresholdingImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);
            List<string> parameters = new List<string>();
            parameters.Add("Threshold");
            DialogBox box = new DialogBox(_mainVM, parameters);
            box.ShowDialog();
            List<double> values = box.GetValues();
            if (values != null)
            {
                byte threshold = (byte)(values[0] + 0.5);
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Thresholding.ThresholdingMethod(GrayInitialImage,
                    threshold);
                    ProcessedImage = Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    // Conversie BGR -> Grayscale
                    GrayProcessedImage = Tools.Convert(ColorInitialImage);
                    GrayProcessedImage = Thresholding.ThresholdingMethod(GrayProcessedImage,
                    threshold);
                    ProcessedImage = Convert(GrayProcessedImage);
                }
            }
        }

        private ICommand _thresholdingOtsuCommand;
        public ICommand ThresholdingOtsuCommand
        {
            get
            {
                if (_thresholdingOtsuCommand == null)
                    _thresholdingOtsuCommand = new RelayCommand(ThresholdingOtsuImage);
                return _thresholdingOtsuCommand;
            }
        }
        private void ThresholdingOtsuImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);
            if (GrayInitialImage != null)
            {
                Tuple<int,int> result= Thresholding.OtsuDoubleThreshold(Utils.ComputeRelativeHistogram(GrayInitialImage));
                int t1= result.Item1;
                int t2 = result.Item2;
                GrayProcessedImage = Thresholding.twoThresholding(GrayInitialImage, t1, t2);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                MessageBox.Show("The image must be grayscale !");
                return;
            }
        }



        #endregion

        #region Filters
        #region Low-Pass filters
        private ICommand _bilateralFilter;
        public ICommand BilateralFilter
        {
            get
            {
                if (_bilateralFilter == null)
                    _bilateralFilter = new RelayCommand(BilateralFilterImage);
                return _bilateralFilter;
            }
        }
        private void BilateralFilterImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);
            List<string> parameters = new List<string>();
            parameters.Add("Sigma 1");
            parameters.Add("Sigma 2");
            DialogBox box = new DialogBox(_mainVM, parameters);
            box.ShowDialog();
            List<double> values = box.GetValues();
            if (values != null)
            {
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Filters.BilateralFilterGray(GrayInitialImage, values[0], values[1]);
                    ProcessedImage = Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                  
                    ColorProcessedImage = Filters.BilateralFilterColor(ColorInitialImage, values[0], values[1]);
                    ProcessedImage = Convert(ColorProcessedImage);
                }
            }
        }
        #endregion

        #region High-Pass filters

        private ICommand _smoothFilterCommand;
        public ICommand SmoothFilterCommand
        {
            get
            {
                if (_smoothFilterCommand == null)
                    _smoothFilterCommand = new RelayCommand(SmoothFilterImage);
                return _smoothFilterCommand;
            }
        }

        private void SmoothFilterImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Filters.GaussianFilter(GrayInitialImage, 1.2);
                ProcessedImage = Convert(GrayProcessedImage);
                }
               
            
        }

        private ICommand _sobelFilterCommand;
        public ICommand SobelFilterCommand
        {
            get
            {
                if (_sobelFilterCommand == null)
                    _sobelFilterCommand = new RelayCommand(SobelFilterImage);
                return _sobelFilterCommand;
            }
        }
        private void SobelFilterImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);
            if (GrayInitialImage != null )
            {
                GrayProcessedImage = Filters.SobelFilter(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
             }

            
        }

        private ICommand _angleImageCommand;
        public ICommand AngleImageCommand
        {
            get
            {
                if (_angleImageCommand == null)
                    _angleImageCommand = new RelayCommand(AngleImage);
                return _angleImageCommand;
            }
        }
        private void AngleImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);
            if (GrayInitialImage != null)
            {
                //GrayInitialImage = Filters.GaussianFilter(GrayInitialImage, 1.2);
                ColorProcessedImage = Filters.ColorEdgesByOrientation(GrayInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }

        private ICommand _nonmaximaSuppressionCommand;
        public ICommand NonmaximaSuppressionCommand
        {
            get
            {
                if (_nonmaximaSuppressionCommand == null)
                    _nonmaximaSuppressionCommand = new RelayCommand(NonmaximaSuppressionImage);
                return _nonmaximaSuppressionCommand;
            }
        }
        private void NonmaximaSuppressionImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Filters.NonmaximaSuppression(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            
        }

        private ICommand _hysteresisThreshldingCommand;
        public ICommand HysteresisThreshldingCommand
        {
            get
            {
                if (_hysteresisThreshldingCommand == null)
                    _hysteresisThreshldingCommand = new RelayCommand(HysteresisThreshldingImage);
                return _hysteresisThreshldingCommand;
            }
        }
        private void HysteresisThreshldingImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            ClearProcessedCanvas(parameter);
            List<string> parameters = new List<string>();
            parameters.Add("T min");
            parameters.Add("T max");
            DialogBox box = new DialogBox(_mainVM, parameters);
            box.ShowDialog();
            List<double> values = box.GetValues();
            if (values != null)
            {
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Filters.HysteresisThresholding(GrayInitialImage, values[0], values[1]);
                    ProcessedImage = Convert(GrayProcessedImage);
                }
            }
        }
        #endregion

        #endregion

        #region Morphological operations
        #endregion

        #region Geometric transformations
        #endregion

        #region Segmentation
        #endregion

        #region Save processed image as original image
        private ICommand _saveAsOriginalImageCommand;
        public ICommand SaveAsOriginalImageCommand
        {
            get
            {
                if (_saveAsOriginalImageCommand == null)
                    _saveAsOriginalImageCommand = new RelayCommand(SaveAsOriginalImage);
                return _saveAsOriginalImageCommand;
            }
        }

        private void SaveAsOriginalImage(object parameter)
        {
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first.");
                return;
            }

            var canvases = (object[])parameter;

            ClearInitialCanvas(canvases[0] as Canvas);

            if (GrayProcessedImage != null)
            {
                GrayInitialImage = GrayProcessedImage;
                InitialImage = Convert(GrayInitialImage);
            }
            else if (ColorProcessedImage != null)
            {
                ColorInitialImage = ColorProcessedImage;
                InitialImage = Convert(ColorInitialImage);
            }

            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion
    }
}