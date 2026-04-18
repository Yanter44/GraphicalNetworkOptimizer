using Microsoft.Xaml.Behaviors;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.MainWindow;
using NetOptimizer.ViewModels.MainWindoww;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NetOptimizer.Behaviors
{
    public class UIElementsDragBehavior : Behavior<FrameworkElement>
    {
        private Point _lastMousePosition;
        private bool _isDragging;
        private FrameworkElement _activeElement;
        private CanvasViewModel canvasVM
        {
            get
            {
                var vm = GetMainVM();
                return vm?.CanvasVM;
            }
        }
        protected override void OnAttached()
        {
            AssociatedObject.MouseDown += OnMouseDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseUp += OnMouseUp;
            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var uiObject = AssociatedObject.DataContext as UIElementBase;
            if (uiObject != null && canvasVM.IsSelecting == false)
            {
                canvasVM.SelectUiObject(uiObject, false);
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDown -= OnMouseDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseUp -= OnMouseUp;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var UIelement = AssociatedObject.DataContext as UIElementBase;
            UIelement.IsSelected = true;
            _isDragging = true;
            _lastMousePosition = e.GetPosition(Application.Current.MainWindow);
            AssociatedObject.CaptureMouse();
            e.Handled = true;
        }
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var uiObject = AssociatedObject.DataContext as UIElementBase;
            if (uiObject != null)
            {
                canvasVM.SelectUiObject(uiObject, true);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;
            var element = AssociatedObject.DataContext as UIElementBase;
            if (element == null) return;

            Point currentMouse = e.GetPosition(Application.Current.MainWindow);

            double dx = currentMouse.X - _lastMousePosition.X;
            double dy = currentMouse.Y - _lastMousePosition.Y;

            double adjustedDx = dx / canvasVM.CanvasScale;
            double adjustedDy = dy / canvasVM.CanvasScale;

            Vector delta = new Vector(adjustedDx, adjustedDy);

            var selectedElements = canvasVM.UIObjects
                .Where(x => x.IsSelected)
                .ToList();

            if (selectedElements.Count > 1)
            {
                foreach (var el in selectedElements)
                {
                    canvasVM.MoveUIElement(el, delta);
                }
            }
            else
            {
                canvasVM.MoveUIElement(element, delta);
            }

            _lastMousePosition = currentMouse;
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
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var UIelement = AssociatedObject.DataContext as UIElementBase;
            UIelement.IsSelected = false;
            if (_isDragging)
            {
                _isDragging = false;
                AssociatedObject.ReleaseMouseCapture();
            }
        }
    }
}
