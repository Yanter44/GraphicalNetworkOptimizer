using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace NetOptimizer.ViewModels.MainWindoww
{
    public class CanvasViewModel : INotifyPropertyChanged
    {
        private Point _canvasOffset;
        private double _canvasScale = 1;

        private Rect _selectionRect;
        private bool _isSelecting;

        private double _canvasWidth;
        private double _canvasHeight;
        public double CanvasWidth { get => _canvasWidth; set { _canvasWidth = value; OnPropertyChanged(); } }
        public double CanvasHeight { get => _canvasHeight; set { _canvasHeight = value; OnPropertyChanged(); } }
        public ObservableCollection<DeviceOnCanvas> Devices { get; set; }  = new ObservableCollection<DeviceOnCanvas>();
        public Point CanvasOffset
        {
            get => _canvasOffset;
            set { _canvasOffset = value; OnPropertyChanged(); }
        }

        public double CanvasScale
        {
            get => _canvasScale;
            set { _canvasScale = value; OnPropertyChanged(); }
        }

        public Rect SelectionRect
        {
            get => _selectionRect;
            set { _selectionRect = value; OnPropertyChanged(); }
        }

        public bool IsSelecting
        {
            get => _isSelecting;
            set { _isSelecting = value; OnPropertyChanged(); }
        }

        public void Zoom(double delta)
        {
            double zoomSpeed = 0.1;
            double newScale = CanvasScale + delta * zoomSpeed;

            if (newScale >= 0.3 && newScale <= 3)
                CanvasScale = newScale;
        }

        public void Pan(Vector delta)
        {
            CanvasOffset = new Point(
                CanvasOffset.X + delta.X,
                CanvasOffset.Y + delta.Y);
        }

        public void StartSelection(Point start)
        {
            SelectionRect = new Rect(start, new Size(0, 0));
            IsSelecting = true;
        }

        public void UpdateSelection(Point start, Point current)
        {
            double x = Math.Min(start.X, current.X);
            double y = Math.Min(start.Y, current.Y);

            double w = Math.Abs(current.X - start.X);
            double h = Math.Abs(current.Y - start.Y);

            SelectionRect = new Rect(x, y, w, h);

            foreach (var device in Devices)
            {
                Rect deviceRect = new Rect(device.X, device.Y, 64, 64);

                device.IsSelected = SelectionRect.IntersectsWith(deviceRect);
            }
        }

        public void StopSelection()
        {
            IsSelecting = false;
            SelectionRect = new Rect();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
