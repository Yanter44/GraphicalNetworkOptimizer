using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NetOptimizer.Behaviors
{
    public class TextBoxBehaviors : Behavior<TextBox>
    {
   
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(Models.Enums.InputType), typeof(TextBoxBehaviors),
                new PropertyMetadata(Models.Enums.InputType.OnlyNumbers));

        public Models.Enums.InputType Mode
        {
            get => (Models.Enums.InputType)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnTextInput;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnTextInput;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        }
        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            string pattern = Mode == Models.Enums.InputType.NumbersAndDots ? "[^0-9.]+" : "[^0-9]+";
            e.Handled = new Regex(pattern).IsMatch(e.Text);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
