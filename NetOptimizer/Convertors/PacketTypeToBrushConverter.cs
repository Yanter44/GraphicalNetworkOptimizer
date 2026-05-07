using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace NetOptimizer.Convertors
{
    public class PacketTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                PacketType.ICMP => Brushes.DarkBlue,
                PacketType.ARP => Brushes.LightGreen,
                PacketType.STP => Brushes.Pink,
                _ => Brushes.Gray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
