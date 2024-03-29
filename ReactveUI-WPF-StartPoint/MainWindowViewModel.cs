using ReactiveUI;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Text;
using System.DirectoryServices.ActiveDirectory;
using System.Windows.Controls;
using System.Windows;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Reactive;
using Splat;
using System.Windows.Input;

namespace ReactveUI_WPF_StartPoint
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string _filePath = "squares 10.tif";
        private const int DEVICE_INDEPENDENT_DPI_X = 96;
        private const int DEVICE_INDEPENDENT_DPI_Y = 96;
        private const double ZOOM_STEPS = 0.1;
        private const double MIN_IMAGE_SIZE = 256;
        private const double MIN_IMAGE_MARGIN = 0.5f;

        private WriteableBitmap _imageSource;
        private TransformGroup _transformGroup;
        private double _actualSizeZoom;
        private double _currentZoom;
        private Size _zoomContainer;
        private bool _mouseDown = false;

        private BitmapDecoder Decoder { get; set; }

        private BitmapFrame _frame;
        private Size _imageContainerSize;
        private string _mousePositionX;
        private string _mousePositionY;
        private string _mouseDPIPositionX;
        private string _mouseDPIPositionY;
        private string _mousePositionGridRelativeToImageX;
        private string _mousePositionGridRelativeToImageY;
        private string _mousePositionImageX;
        private string _mousePositionImageY;
        private string _mousePositionImageRelativeToGridX;
        private string _mousePositionImageRelativeToGridY;
        private double _onDemandXPosition;
        private double _onDemandYPosition;
        private string _mouseEnterPositionX;
        private string _mouseEnterPositionY;
        private string _mouseLeavePositionX;
        private string _mouseLeavePositionY;

        public WriteableBitmap ImageSource { get => _imageSource; set => this.RaiseAndSetIfChanged(ref _imageSource, value); }
        public TransformGroup TransformGroup { get => _transformGroup; set => this.RaiseAndSetIfChanged(ref _transformGroup, value); }

        public string MousePositionGridX { get => _mousePositionX; set => this.RaiseAndSetIfChanged(ref _mousePositionX, value); }
        public string MousePositionGridY { get => _mousePositionY; set => this.RaiseAndSetIfChanged(ref _mousePositionY, value); }
        public string MousePositionGridRelativeToImageX {  get => _mousePositionGridRelativeToImageX; set => this.RaiseAndSetIfChanged(ref _mousePositionGridRelativeToImageX, value); }
        public string MousePositionGridRelativeToImageY { get => _mousePositionGridRelativeToImageY; set => this.RaiseAndSetIfChanged(ref _mousePositionGridRelativeToImageY, value); }

        public string MousePositionImageX { get => _mousePositionImageX; set => this.RaiseAndSetIfChanged(ref _mousePositionImageX, value); }
        public string MousePositionImageY { get => _mousePositionImageY; set => this.RaiseAndSetIfChanged(ref _mousePositionImageY, value); }

        public string MousePositionImageRelativeToGridX { get => _mousePositionImageRelativeToGridX; set => this.RaiseAndSetIfChanged(ref _mousePositionImageRelativeToGridX, value); }
        public string MousePositionImageRelativeToGridY { get => _mousePositionImageRelativeToGridY; set => this.RaiseAndSetIfChanged(ref _mousePositionImageRelativeToGridY, value); }




        public string MouseDPIPositionX { get => _mouseDPIPositionX; set => this.RaiseAndSetIfChanged(ref _mouseDPIPositionX, value); }
        public string MouseDPIPositionY { get => _mouseDPIPositionY; set => this.RaiseAndSetIfChanged(ref _mouseDPIPositionY, value); }
        public double OnDemandXPosition { get => _onDemandXPosition; set => this.RaiseAndSetIfChanged(ref _onDemandXPosition, value); }
        public double OnDemandYPosition { get => _onDemandYPosition; set => this.RaiseAndSetIfChanged(ref _onDemandYPosition, value); }

        public string MouseEnterPositionX {  get => _mouseEnterPositionX; set => this.RaiseAndSetIfChanged(ref _mouseEnterPositionX, value); }
        public string MouseEnterPositionY { get => _mouseEnterPositionY; set => this.RaiseAndSetIfChanged(ref _mouseEnterPositionY, value); }
        public string MouseLeavePositionX { get => _mouseLeavePositionX; set => this.RaiseAndSetIfChanged(ref _mouseLeavePositionX, value); }
        public string MouseLeavePositionY { get => _mouseLeavePositionY; set => this.RaiseAndSetIfChanged(ref _mouseLeavePositionY, value); }
                 

        public double CurrentZoom { get => _currentZoom; set => this.RaiseAndSetIfChanged(ref _currentZoom, value); }

        public MainWindowViewModel()
        {
            LoadFile();
            TransformGroup = new TransformGroup();
            TransformGroup.Children.Add(new ScaleTransform(1, 1));
            TransformGroup.Children.Add(new TranslateTransform(0, 0));
        }

        private void MouseMove(Point currentPosition)
        {
            if (_mouseDown)
            {
                //Vector v = _startPosition - currentPosition;
                //var translateX = _origin.X - v.X;
                //var translateY = _origin.Y - v.Y;

                //AutoTransform(0, 0, 0, 0, translateX, translateY);
            }
        }

        //private void ImageMouseMove(Point currentPosition)
        //{
        //    MousePosition = $"X: {currentPosition.X}, Y: {currentPosition.Y}";
        //    var dpiRelatedPoint = GetDPIRelatedPoint(currentPosition);
        //    MouseDPIPosition = $"X: {dpiRelatedPoint.X}, Y: {dpiRelatedPoint.Y}";

        //    System.Diagnostics.Debug.WriteLine(MouseDPIPosition);
        //    System.Diagnostics.Debug.WriteLine(MousePosition);
        //}

        private void LoadFile()
        {
            using (var imageStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Decoder = new TiffBitmapDecoder(imageStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                _frame = Decoder.Frames[0];

                ImageSource = new WriteableBitmap(_frame);
            }

            _actualSizeZoom = _frame.DpiX / DEVICE_INDEPENDENT_DPI_X;
            _currentZoom = 1;

            _zoomContainer = new Size(_frame.Width, _frame.Height);
        }

        private void FitZoom()
        {
            if (ImageSource == null)
            {
                return;
            }

            var deltaZoom = Math.Min(_imageContainerSize.Height / _zoomContainer.Height, _imageContainerSize.Width / _zoomContainer.Width) - _currentZoom;
            while ((_currentZoom + deltaZoom) * DEVICE_INDEPENDENT_DPI_X / ImageSource.DpiX > 8)
            {
                deltaZoom -= ZOOM_STEPS;
            }
            var newZoom = _currentZoom + deltaZoom;
            var translateX = (_imageContainerSize.Width - newZoom * _zoomContainer.Width) / 2.0;
            var translateY = (_imageContainerSize.Height - newZoom * _zoomContainer.Height) / 2.0;

            translateX = Math.Round(translateX, 4);
            translateY = Math.Round(translateY, 4);

            AutoTransform(0, 0, deltaZoom, deltaZoom, translateX, translateY);
        }

        internal void NotifyImageContainerSizeChanged(SizeChangedEventArgs eventArgs)
        {
            _imageContainerSize = eventArgs.NewSize;

            FitZoom();
        }

        private void AutoTransform(double originX, double originY, double deltaScaleX, double deltaScaleY, double translateX, double translateY)
        {
            if (TransformGroup == null)
            {
                return;
            }

            var scaleTransform = GetScaleTransform();
            var translateTransform = GetTranslateTransform();

            scaleTransform.CenterX = originX;
            scaleTransform.CenterY = originY;

            //auto limit zoom if image is smaller than container
            //auto limits zoom is image scale is larger than 800%
            //else respect request
            if (_zoomContainer.Width * (scaleTransform.ScaleX + deltaScaleX) < MIN_IMAGE_SIZE &&
                _zoomContainer.Height * (scaleTransform.ScaleY + deltaScaleY) < MIN_IMAGE_SIZE)
            {
                return;
            }
            else if ((scaleTransform.ScaleX + deltaScaleX) * DEVICE_INDEPENDENT_DPI_X / ImageSource.DpiX > 8)
            {
                return;
            }
            else
            {
                scaleTransform.ScaleX += deltaScaleX;
                scaleTransform.ScaleY += deltaScaleY;
            }

            //auto limit placement away from the container center
            if (translateX > (1 - MIN_IMAGE_MARGIN) * _imageContainerSize.Width)
            {
                translateX = (1 - MIN_IMAGE_MARGIN) * _imageContainerSize.Width;
            }
            if (translateY > (1 - MIN_IMAGE_MARGIN) * _imageContainerSize.Height)
            {
                translateY = (1 - MIN_IMAGE_MARGIN) * _imageContainerSize.Height;
            }
            if (_zoomContainer.Width * scaleTransform.ScaleX + translateX < MIN_IMAGE_MARGIN * _imageContainerSize.Width)
            {
                translateX = MIN_IMAGE_MARGIN * _imageContainerSize.Width - _zoomContainer.Width * scaleTransform.ScaleX;
            }
            if (_zoomContainer.Height * scaleTransform.ScaleY + translateY < MIN_IMAGE_MARGIN * _imageContainerSize.Height)
            {
                translateY = MIN_IMAGE_MARGIN * _imageContainerSize.Height - _zoomContainer.Height * scaleTransform.ScaleY;
            }

            //auto center if image is smaller than container, else respect request
            if (ImageSource.Width * scaleTransform.ScaleX <= _imageContainerSize.Width && _zoomContainer.Height * scaleTransform.ScaleY <= _imageContainerSize.Height)
            {
                translateTransform.X = (_imageContainerSize.Width - scaleTransform.ScaleX * _zoomContainer.Width) / 2.0;
                translateTransform.Y = (_imageContainerSize.Height - scaleTransform.ScaleY * _zoomContainer.Height) / 2.0;
            }
            else
            {
                translateTransform.X = translateX;
                translateTransform.Y = translateY;
            }

            CurrentZoom = scaleTransform.ScaleX;
        }

        private TranslateTransform GetTranslateTransform()
        {
            return (TranslateTransform)TransformGroup?.Children.First(tg => tg is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform()
        {
            return (ScaleTransform)TransformGroup?.Children.First(tg => tg is ScaleTransform);
        }

        public Point GetDPIRelatedPoint(Point point)
        {
            var dpiX = DEVICE_INDEPENDENT_DPI_X / _frame.DpiX;
            var dpiY = DEVICE_INDEPENDENT_DPI_Y / _frame.DpiY;

            var actualX = point.X / dpiX;
            var actualY = point.Y / dpiY;

            return new Point(actualX, actualY);
        }

        public void ImageMouseMove(Point currentPosition)
        {
            //MousePositionX = $"X: {currentPosition.X}";
            //MousePositionY = $"Y: {currentPosition.Y}";
            //var dpiRelatedPoint = GetDPIRelatedPoint(currentPosition);
            //MouseDPIPositionX = $"X: {dpiRelatedPoint.X}";
            //MouseDPIPositionY = $"Y: {dpiRelatedPoint.Y}";
        }

        public void RenderGridMouseMoveEventForGrid(Point currentPosition)
        {
            MousePositionGridX = $"X: {currentPosition.X}";
            MousePositionGridY = $"Y: {currentPosition.Y}";
            var dpiRelatedPoint = GetDPIRelatedPoint(currentPosition);
            //MouseDPIPositionX = $"X: {dpiRelatedPoint.X}";
            //MouseDPIPositionY = $"Y: {dpiRelatedPoint.Y}";
        }
        
        public void RenderGridMouseMoveEventForImageSource(Point currentPosition)
        {
            MousePositionGridRelativeToImageX = $"X: {currentPosition.X}";
            MousePositionGridRelativeToImageY = $"Y: {currentPosition.Y}";
            var dpiRelatedPoint = GetDPIRelatedPoint(currentPosition);
            //MouseDPIPositionX = $"X: {dpiRelatedPoint.X}";
            //MouseDPIPositionY = $"Y: {dpiRelatedPoint.Y}";
        }

        public void ImageMouseMoveEventForImage(Point point)
        {
            MousePositionImageX = $"X: {point.X}";
            MousePositionImageY = $"Y: {point.Y}";
        }

        public void ImageMouseMoveEventForGrid(Point point)
        {
            MousePositionImageRelativeToGridX = $"X: {point.X}";
            MousePositionImageRelativeToGridY = $"Y: {point.Y}";
        }

        internal void ImageMouseDownEventForImage(object value, object sender)
        {
            var image = sender as UIElement;

            if (image == null)
            {
                return;
            }

            var position = Mouse.GetPosition(image);

            OnDemandXPosition = position.X;
            OnDemandYPosition = position.Y;
        }

        internal void ImageMouseEnterEventForImage(Point position)
        {


            MouseEnterPositionX = $"X: {position.X}";
            MouseEnterPositionY = $"Y: {position.Y}";
        }

        internal void ImageMouseLeaveEventForImage(Point position)
        {
            MouseLeavePositionX = $"X: {position.X}";
            MouseLeavePositionY = $"Y: {position.Y}";
        }
    }
}
