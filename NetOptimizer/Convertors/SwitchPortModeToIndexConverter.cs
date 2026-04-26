using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NetOptimizer.Convertors
{
    class SwitchPortModeToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SwitchPortMode switchport)
            {
                if(switchport == SwitchPortMode.Access)
                {
                    return 0;
                }
                else { return 1; }
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                return i == 0
                    ? SwitchPortMode.Access
                    : SwitchPortMode.Trunk; 
            }
            return SwitchPortMode.Access;
        }
    }
}
