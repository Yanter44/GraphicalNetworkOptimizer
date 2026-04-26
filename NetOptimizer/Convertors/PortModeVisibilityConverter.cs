using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace NetOptimizer.Convertors
{
    public class PortModeVisibilityConverter : IMultiValueConverter 
    { 
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        { 
            if (values.Length < 2)
                return Visibility.Collapsed;

            if (values[0] is SwitchPortMode currentMode && values[1] is string targetMode) 
            { 
                return currentMode.ToString() == targetMode ? Visibility.Visible : Visibility.Collapsed;
            } 

            return Visibility.Collapsed; 
        } 
        
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException(); }
}
