using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;

namespace NetOptimizer.Convertors
{
    public class CanvasCursorModeToCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not CanvasCursorMode mode)
                return Cursors.Arrow;

            return mode switch
            {
                CanvasCursorMode.Draw => Cursors.Pen,
                CanvasCursorMode.Connect => Cursors.Cross,
                CanvasCursorMode.SendPacket => Cursors.Cross,
                _ => Cursors.Arrow
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
