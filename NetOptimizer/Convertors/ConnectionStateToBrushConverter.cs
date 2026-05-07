using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace NetOptimizer.Convertors
{
    public class ConnectionStateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                PointConnectionState.Connected => Brushes.LimeGreen,
                PointConnectionState.Negotiating => Brushes.Gold,
                PointConnectionState.Error => Brushes.Red,
                _ => Brushes.Gray
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
