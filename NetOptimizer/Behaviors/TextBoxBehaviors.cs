using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NetOptimizer.Behaviors
{
    public static class TextBoxBehaviors
    {
        public static DependencyProperty IsNumericProperty = DependencyProperty
             .RegisterAttached("IsNumeric", typeof(bool), typeof(TextBoxBehaviors), new PropertyMetadata(false, CheckAndSubscribeToMethod));

        public static void SetIsNumeric(UIElement element, bool value) => element.SetValue(IsNumericProperty, value);
        public static bool GetIsNumeric(UIElement element) => (bool)element.GetValue(IsNumericProperty);
        public static void CheckAndSubscribeToMethod(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox tb)
            {
                if ((bool)e.NewValue) 
                    tb.PreviewTextInput += CheckWhatCharIsNumeric;
                else
                    tb.PreviewTextInput -= CheckWhatCharIsNumeric;
            }

        }
        private static void CheckWhatCharIsNumeric(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox tb)
            {
                var textToInt = int.TryParse(e.Text, out int result);
                if (!textToInt)
                {
                    e.Handled = true;
                }
                else { e.Handled = false; }
            }
        }
    }
}
