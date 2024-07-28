using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using FullStackWork.Models;
using Emgu.CV;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Rectangle = System.Windows.Shapes.Rectangle;
using Point = System.Windows.Point;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Net;
using OpenCvSharp.Gpu;
using System.Drawing;
using Brushes = System.Windows.Media.Brushes;
using FullStackWork.Store;

namespace FullStackWork.ViewModels
{
    public class VideoPageVM : ViewModelBase
    {
        #region UI variable
        private string _imagePath = "";
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
                OnPropertyChanged("ImagePath");
            }
        }
        private ObservableCollection<ImageData> _videoDataList = new ObservableCollection<ImageData>();
        public ObservableCollection<ImageData> VideoDataList
        {
            get
            {
                return this._videoDataList;
            }
            set
            {
                this._videoDataList = value;
            }
        }
        private ObservableCollection<ImageData> _imageDataList = new ObservableCollection<ImageData>();
        public ObservableCollection<ImageData> ImageDataList
        {
            get
            {
                return this._imageDataList;
            }
            set
            {
                this._imageDataList = value;
            }
        }
        private int _selectIndex = -1;
        public int SelectIndex
        {
            get
            {
                return this._selectIndex;
            }
            set
            {
                this._selectIndex = value;
                if (SelectIndex >= 0)
                    ImagePath = VideoDataList[SelectIndex].filepath;
            }
        }
        public Canvas canvas { get; set; }
        public Rectangle rectangle { get; set; }
        #endregion
        #region UI Command
        public RelayCommand LoadVideoCommand { get; private set; }
        public RelayCommand<object> DeleteImageCMD { get; private set; }
        public RelayCommand<object> CmdSizeChanged { get; private set; }
        public RelayCommand ClearCommand { get; private set; }
        public RelayCommand CropCommand { get; private set; }
        public RelayCommand<MouseEventArgs> CmdLeftMouseDown { get; private set; }
        public RelayCommand<MouseEventArgs> CmdLeftMouseMove { get; private set; }
        public RelayCommand<MouseEventArgs> CmdLeftMouseUp { get; private set; }
        #endregion
        #region VM variable
        private bool IsDrawing = false;
        private Point StartPoint = new Point(0, 0);
        private Point EndPoint = new Point(0, 0);
        private List<Point> Contour = new List<Point>();
        private double UIWidth = 0;
        private double UIHeight = 0;
        private readonly string RootFile = System.Windows.Forms.Application.StartupPath;
        #endregion
        public VideoPageVM()
        {
            LoadVideoCommand = new RelayCommand(LoadVideo);
            DeleteImageCMD = new RelayCommand<object>(DeleteImage);
            CmdSizeChanged = new RelayCommand<object>(Redraw);
            ClearCommand = new RelayCommand(ClearDrawing);
            CropCommand = new RelayCommand(CropImage);
            CmdLeftMouseDown = new RelayCommand<MouseEventArgs>(LeftMouseDown);
            CmdLeftMouseMove = new RelayCommand<MouseEventArgs>(LeftMouseMove);
            CmdLeftMouseUp = new RelayCommand<MouseEventArgs>(LeftMouseUp);
            canvas = new Canvas();
            rectangle = new Rectangle();
        }
        private void LoadVideo()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.Multiselect = true;
            dialog.Filter = "(*.mp4)|*.mp4;";
            dialog.ShowDialog();

            foreach (string FileName in dialog.FileNames)
            {
                string ImageName = FileName.Split("\\").Last();
                ImageData NewImage = new ImageData(FileName, ImageName);
                VideoDataList.Add(NewImage);
            }
        }
        private void DeleteImage(object? parameter)
        {
            if (parameter == null) return;
            ImageData imageData = (ImageData)parameter;
            VideoDataList.Remove(imageData);
        }
        public void LeftMouseDown(MouseEventArgs e)
        {
            if (canvas.Children.Count != 0) canvas.Children.Clear();
            if (ImagePath == null || IsDrawing) return;

            IsDrawing = true;

            if (e.OriginalSource is Canvas)
            {
                canvas = (Canvas)e.OriginalSource;
                UIWidth = canvas.ActualWidth;
                UIHeight = canvas.ActualHeight;
                StartPoint = e.GetPosition((IInputElement)e.Source);
            }
            rectangle = CreateTempRect();
            canvas.Children.Add(rectangle);
        }
        private void LeftMouseMove(MouseEventArgs e)
        {
            if (ImagePath == null || !IsDrawing) return;
            if (e.OriginalSource is Canvas)
            {
                EndPoint = e.GetPosition((IInputElement)e.Source);
            }
            UpdateRectangle();
        }
        private void LeftMouseUp(MouseEventArgs e)
        {
            if (ImagePath == null || !IsDrawing) return;
            IsDrawing = false;
            AddRectangle();
        }
        #region auxiliary func
        private Rectangle CreateTempRect()
        {
            Rectangle rect = new Rectangle
            {
                Width = 0,
                Height = 0,
                Stroke = Brushes.IndianRed,
                StrokeThickness = 2,
                Fill = Brushes.Transparent,
                IsHitTestVisible = true
            };
            Canvas.SetLeft(rect, StartPoint.X);
            Canvas.SetTop(rect, StartPoint.Y);

            return rect;
        }
        private void UpdateRectangle()
        {
            AddRectangle(true);
            return;
        }

        private void AddRectangle(bool isUpdate = false)
        {
            if (!IsDrawing) return;
            try
            {
                double width = (EndPoint.X - StartPoint.X);
                double height = EndPoint.Y - StartPoint.Y;
                double left = StartPoint.X;
                double top = StartPoint.Y; ;
                if (width < 0)
                {
                    left = EndPoint.X;
                    width = -width;
                }
                if (height < 0)
                {
                    top = EndPoint.Y;
                    height = -height;
                }
                rectangle.Width = width;
                rectangle.Height = height;
                Canvas.SetLeft(rectangle, left);
                Canvas.SetTop(rectangle, top);
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void ClearDrawing()
        {
            canvas.Children.Clear();
        }
        private void CropImage()
        {
            //Mat NowImage = new Mat(ImagePath);
            //Mat CropImage = new Mat(NowImage, new System.Drawing.Rectangle((int)(StartPoint.X * NowImage.Width / UIWidth), (int)(StartPoint.Y * NowImage.Height / UIHeight), (int)(rectangle.Width * NowImage.Width / UIWidth), (int)(rectangle.Height * NowImage.Height / UIHeight)));
            //string CropName = ImagePath.Split("\\").Last().Split('.')[0] + "_Copy.jpg";
            //CropImage.Save(RootFile + "\\" + CropName);
            //ImageData NewCropImage = new ImageData(RootFile + "\\" + CropName, CropName);
            //if (!ImageDataList.Contains(NewCropImage))
            //    ImageDataList.Add(NewCropImage);
        }
        private void Redraw(object? parameter)
        {
            if (rectangle != null)
            {
                double WidthRate = canvas.ActualWidth / UIWidth;
                double HeightRate = canvas.ActualHeight / UIHeight;
                double left = StartPoint.X * WidthRate;
                double top = StartPoint.Y * HeightRate;

                rectangle.Width *= WidthRate;
                rectangle.Height *= HeightRate;
                Canvas.SetLeft(rectangle, left);
                Canvas.SetTop(rectangle, top);
            }

            UIHeight = canvas.ActualHeight;
            UIWidth = canvas.ActualWidth;
        }
        #endregion
    }
}
