using Microsoft.Xaml.Behaviors;
using NetOptimizer.Events;
using NetOptimizer.Models;
using NetOptimizer.Models.Enums;
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
        public ICommand DeviceMouseDownCommand
        {
            get => (ICommand)GetValue(DeviceMouseDownCommandProperty);
            set => SetValue(DeviceMouseDownCommandProperty, value);
        }

        public static readonly DependencyProperty DeviceMouseDownCommandProperty = DependencyProperty.Register(nameof(DeviceMouseDownCommand),
                                                                                                               typeof(ICommand),
                                                                                                               typeof(DeviceDragBehavior));
        public ICommand MoveDeviceCommand
        {
            get => (ICommand)GetValue(MoveDeviceCommandProperty);
            set => SetValue(MoveDeviceCommandProperty, value);
        }

        public static readonly DependencyProperty MoveDeviceCommandProperty =
            DependencyProperty.Register(
                nameof(MoveDeviceCommand),
                typeof(ICommand),
                typeof(DeviceDragBehavior));
        public double CanvasScale { get => (double)GetValue(CanvasScaleProperty); set => SetValue(CanvasScaleProperty, value); }

        public static readonly DependencyProperty CanvasScaleProperty = DependencyProperty.Register(nameof(CanvasScale), 
                                                                                                    typeof(double), typeof(DeviceDragBehavior));
        public ICommand DeviceMouseOverCommand { get => (ICommand)GetValue(DeviceMouseOverProperty); set => SetValue(DeviceMouseOverProperty, value); }
        public static readonly DependencyProperty DeviceMouseOverProperty = DependencyProperty.Register(nameof(DeviceMouseOverCommand),
                                                                                                        typeof(ICommand), typeof(DeviceDragBehavior));
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
            if (device == null) return;

            DeviceMouseOverCommand?.Execute(new DeviceMouseEventArgs
            {
                Device = device,
                Action = DeviceMouseAction.MouseEnter
            });
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var device = AssociatedObject.DataContext as DeviceOnCanvas;
            if (device == null) return;

            DeviceMouseOverCommand?.Execute(new DeviceMouseEventArgs
            {
                Device = device,
                Action = DeviceMouseAction.MouseLeave
            });
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            var device = element?.DataContext as DeviceOnCanvas;

            if (device == null) return;

            if (e.ClickCount == 2)
            {
                DeviceMouseDownCommand?.Execute(new DeviceMouseEventArgs
                {
                    Device = device,
                    Action = DeviceMouseAction.DoubleClick
                });
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                _activeElement = element;
                _lastMousePosition = e.GetPosition(_activeElement.Parent as Canvas);
                _activeElement.CaptureMouse();

                DeviceMouseDownCommand?.Execute(new DeviceMouseEventArgs
                {
                    Device = device,
                    Action = DeviceMouseAction.SingleClick,
                    Position = e.GetPosition(null)
                });
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _activeElement == null) return;

            var device = _activeElement.DataContext as DeviceOnCanvas;
            if (device == null) return;

            Point currentMouse = e.GetPosition(_activeElement.Parent as Canvas);
            double dx = currentMouse.X - _lastMousePosition.X;
            double dy = currentMouse.Y - _lastMousePosition.Y;

            double adjustedDx = dx / CanvasScale;
            double adjustedDy = dy / CanvasScale;
            
            MoveDeviceCommand?.Execute(new DeviceMoveEventArgs
            {
                Device = device,
                Dx = adjustedDx,
                Dy = adjustedDy
            });
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
