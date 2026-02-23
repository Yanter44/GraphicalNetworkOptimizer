using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NetOptimizer.Convertors
{
    public class BooleanToBrushConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
                return Brushes.DodgerBlue; // Цвет рамки при выделении

            return Brushes.Transparent; // Цвет когда не выделено
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }
}
