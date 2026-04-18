using Microsoft.Xaml.Behaviors;
using NetOptimizer.Models;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.MainWindow;
using NetOptimizer.ViewModels.MainWindoww;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace NetOptimizer.Behaviors
{
    public class DeviceDragBehavior : Behavior<FrameworkElement>
    {
        private Point _clickPosition;
        private bool _isDragging;
        private FrameworkElement _activeElement;
        private Point _lastMousePosition;
        private CanvasViewModel сanvasVM { get { var vm = GetMainVM(); return vm?.CanvasVM; } }
        protected override void OnAttached()
        {
            AssociatedObject.MouseDown += OnMouseDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseUp += OnMouseUp;
            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        private MainWindowViewModel GetMainVM()
        {
            DependencyObject parent = AssociatedObject;
            while (parent != null)
            {
                if (parent is FrameworkElement fe && fe.DataContext is MainWindowViewModel vm)
                    return vm;

                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var device = AssociatedObject.DataContext as DeviceOnCanvas;
            if (device != null)
            {
                сanvasVM.SelectDevice(device, true);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var device = AssociatedObject.DataContext as DeviceOnCanvas;
            if (device != null && сanvasVM.IsSelecting == false)
            {
                сanvasVM.SelectDevice(device, false);
            }

        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                var element = sender as FrameworkElement;
                var device = element?.DataContext as DeviceOnCanvas;
                if (device == null) return;
                сanvasVM.ShowDeviceParametrsWindow(device);
            } 
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                _activeElement = sender as FrameworkElement;
                _lastMousePosition = e.GetPosition(_activeElement.Parent as Canvas);
                _activeElement.CaptureMouse();
                e.Handled = true;
            }
        }
      
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _activeElement == null) return;

            var device = _activeElement.DataContext as DeviceOnCanvas;
            var vm = GetMainVM();
            if (device == null || vm == null) return;

            Point currentMouse = e.GetPosition(_activeElement.Parent as Canvas);

            double dx = currentMouse.X - _lastMousePosition.X;
            double dy = currentMouse.Y - _lastMousePosition.Y;

            double adjustedDx = dx / vm.CanvasVM.CanvasScale;
            double adjustedDy = dy / vm.CanvasVM.CanvasScale;

            var selectedDevices = vm.CanvasVM.Devices
                .Where(x => x.IsSelected)
                .ToList();

            if (selectedDevices.Count > 1)
            {
                foreach (var dev in selectedDevices)
                {
                    vm.CanvasVM.MoveDevice(dev, adjustedDx, adjustedDy);
                }
            }
            else
            {
                vm.CanvasVM.MoveDevice(device, adjustedDx, adjustedDy);
            }
            _lastMousePosition = currentMouse;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _activeElement?.ReleaseMouseCapture();
            _activeElement = null;
        }
    }
}
