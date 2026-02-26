using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NetOptimizer.Behaviors
{
    public static class NumericMaxTextBoxSymblosBehavior
    {
        public static readonly DependencyProperty MaxNumbersProperty =
            DependencyProperty.RegisterAttached(
                "MaxNumbers",
                typeof(int),
                typeof(NumericMaxTextBoxSymblosBehavior),
                new PropertyMetadata(0, OnMaxNumbersChanged));

        public static void SetMaxNumbers(DependencyObject element, int value)
            => element.SetValue(MaxNumbersProperty, value);

        public static int GetMaxNumbers(DependencyObject element)
            => (int)element.GetValue(MaxNumbersProperty);

        private static void OnMaxNumbersChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox tb)
            {
                if ((int)e.NewValue > 0)
                    tb.PreviewTextInput += CheckMaxSize;
                else
                    tb.PreviewTextInput -= CheckMaxSize;
            }
        }

        private static void CheckMaxSize(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox tb)
            {
                int maxSize = GetMaxNumbers(tb);

                string futureText =
                    tb.Text.Remove(tb.SelectionStart, tb.SelectionLength)
                           .Insert(tb.SelectionStart, e.Text);

                if (futureText.Length > maxSize)
                    e.Handled = true;
            }
        }
    }
}

