using NetOptimizer.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NetOptimizer.Behaviors
{
    public static class ScrollBarWheelDirectionBehavior
    {
        public static readonly DependencyProperty OrientationScrollProperty =
            DependencyProperty.RegisterAttached("OrientationScroll",
                typeof(MouseOrientationScrollType),
                typeof(ScrollBarWheelDirectionBehavior),
                new PropertyMetadata(MouseOrientationScrollType.None, OnPriorityChanged));

        public static MouseOrientationScrollType GetOrientationScroll(DependencyObject obj)
        {
            return (MouseOrientationScrollType)obj.GetValue(OrientationScrollProperty);
        }

        public static void SetOrientationScroll(DependencyObject obj, MouseOrientationScrollType value)
        {
            obj.SetValue(OrientationScrollProperty, value);
        }

        private static void OnPriorityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                scrollViewer.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;

                var mode = (MouseOrientationScrollType)e.NewValue;
                if (mode != MouseOrientationScrollType.None)
                {
                    scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
                }
            }
        }

        private static void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                var mode = (MouseOrientationScrollType)scrollViewer.GetValue(OrientationScrollProperty);
                if (mode == MouseOrientationScrollType.Horizontal)
                {
                    if (e.Delta > 0) scrollViewer.LineLeft();
                    else scrollViewer.LineRight();
                    e.Handled = true;
                }
                else if (mode == MouseOrientationScrollType.Vertical)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                    e.Handled = true;
                }
            }
        }
    }

}
