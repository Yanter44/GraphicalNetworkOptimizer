using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NetOptimizer.Behaviors
{
    public static class ScrollBarWheelDirectionBehavior
    {
        public static readonly DependencyProperty OrientationScrollProperty =
            DependencyProperty.RegisterAttached(
                "OrientationScroll",
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
            if (d is ScrollViewer sv)
            {
                sv.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;

                if ((MouseOrientationScrollType)e.NewValue != MouseOrientationScrollType.None)
                {
                    sv.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
                }
            }
        }

        private class ScrollState
        {
            public double Target;
            public bool IsAnimating;
            public bool IsHorizontal;
        }

        private static readonly Dictionary<ScrollViewer, ScrollState> _states = new();

        private static ScrollState GetState(ScrollViewer sv)
        {
            if (!_states.TryGetValue(sv, out var state))
            {
                state = new ScrollState();
                _states[sv] = state;
            }
            return state;
        }

        private static void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not ScrollViewer sv) return;

            var mode = GetOrientationScroll(sv);
            if (mode == MouseOrientationScrollType.None) return;

            var state = GetState(sv);
            double delta = e.Delta;

            if (mode == MouseOrientationScrollType.Vertical)
            {
                state.IsHorizontal = false;

                state.Target += -delta * 0.5;

                state.Target = Clamp(
                    state.Target,
                    0,
                    sv.ExtentHeight - sv.ViewportHeight
                );

                StartAnimation(sv, state);
                e.Handled = true;
            }
            else if (mode == MouseOrientationScrollType.Horizontal)
            {
                state.IsHorizontal = true;

                state.Target += -delta * 0.5;

                state.Target = Clamp(
                    state.Target,
                    0,
                    sv.ExtentWidth - sv.ViewportWidth
                );

                StartAnimation(sv, state);
                e.Handled = true;
            }
        }

        private static void StartAnimation(ScrollViewer sv, ScrollState state)
        {
            if (state.IsAnimating)
                return;

            state.IsAnimating = true;

            void OnRendering(object sender, EventArgs e)
            {
                double current = state.IsHorizontal
                    ? sv.HorizontalOffset
                    : sv.VerticalOffset;

                double diff = state.Target - current;

                if (Math.Abs(diff) < 0.5)
                {
                    if (state.IsHorizontal)
                        sv.ScrollToHorizontalOffset(state.Target);
                    else
                        sv.ScrollToVerticalOffset(state.Target);

                    CompositionTarget.Rendering -= OnRendering;
                    state.IsAnimating = false;
                    return;
                }

                double step = diff * 0.15;

                if (state.IsHorizontal)
                    sv.ScrollToHorizontalOffset(current + step);
                else
                    sv.ScrollToVerticalOffset(current + step);
            }

            CompositionTarget.Rendering += OnRendering;
        }

        private static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}