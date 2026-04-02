using Microsoft.Xaml.Behaviors;
using NetOptimizer.Models.UIElements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NetOptimizer.Behaviors
{
        public class UIElementsDragBehavior : Behavior<FrameworkElement>
        {
            private Point _lastMousePosition;
            private bool _isDragging;

            protected override void OnAttached()
            {
                AssociatedObject.MouseDown += OnMouseDown;
                AssociatedObject.MouseMove += OnMouseMove;
                AssociatedObject.MouseUp += OnMouseUp;
            }

            protected override void OnDetaching()
            {
                AssociatedObject.MouseDown -= OnMouseDown;
                AssociatedObject.MouseMove -= OnMouseMove;
                AssociatedObject.MouseUp -= OnMouseUp;
            }

            private void OnMouseDown(object sender, MouseButtonEventArgs e)
            {
                // Берем DataContext (нашу вью-модель элемента)
                if (AssociatedObject.DataContext is UIElementBase)
                {
                    _isDragging = true;
                    _lastMousePosition = e.GetPosition(Application.Current.MainWindow);
                    AssociatedObject.CaptureMouse();
                    e.Handled = true; // Чтобы клик не провалился в канвас под фигурой
                }
            }

            private void OnMouseMove(object sender, MouseEventArgs e)
            {
                if (!_isDragging) return;

                Point currentMousePosition = e.GetPosition(Application.Current.MainWindow);
                Vector delta = currentMousePosition - _lastMousePosition;

                var element = AssociatedObject.DataContext as UIElementBase;

                if (element != null)
                {
                    // 1. Двигаем базовые координаты (X, Y)
                    element.X += delta.X;
                    element.Y += delta.Y;

                    // 2. Если это Линия, нужно подвинуть и её точки
                    if (element is LineElement line)
                    {
                        line.Start = new Point(line.Start.X + delta.X, line.Start.Y + delta.Y);
                        line.End = new Point(line.End.X + delta.X, line.End.Y + delta.Y);
                    }

                    // 3. Если это кривая (Polyline), двигаем все её точки
                    if (element is CurveElement curve && curve.Points != null)
                    {
                        for (int i = 0; i < curve.Points.Count; i++)
                        {
                            var p = curve.Points[i];
                            curve.Points[i] = new Point(p.X + delta.X, p.Y + delta.Y);
                        }
                    }
                }

                _lastMousePosition = currentMousePosition;
            }

            private void OnMouseUp(object sender, MouseButtonEventArgs e)
            {
                if (_isDragging)
                {
                    _isDragging = false;
                    AssociatedObject.ReleaseMouseCapture();
                }
            }
        }
}
