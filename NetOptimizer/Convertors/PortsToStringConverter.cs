using Microsoft.VisualBasic;
using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NetOptimizer.Convertors
{
    public class PortsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ports = value as IEnumerable<Port>;

            if (ports == null || !ports.Any()) return null;

            return string.Join(", ", ports
                .GroupBy(p => p.PortName)
                .Select(g => $"{g.Key} ({g.Count()})"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
