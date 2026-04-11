using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace NetOptimizer.Convertors
{
    public class SelectionToEffectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
            {
                return new DropShadowEffect
                {
                    BlurRadius = 5,
                    ShadowDepth = 0, 
                    Opacity = 0.4,
                    Color = Colors.Gray
                };
            }

            return new DropShadowEffect
            {
                BlurRadius = 10,
                Opacity = 0.1,
                Direction = 275,
                Color = Colors.LightGray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
