using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NetOptimizer.Convertors
{
    public class PortHeaderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is bool isLinked &&
                values[1] is Port p)
            {
                return $"{(isLinked ? "●" : "○")} {p.PortName} {p.PortNumber} ({(isLinked ? "Linked" : "Not Linked")})";
            }

            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
