using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace NetOptimizer.Convertors
{
    public class CanvasCursorIsDrawingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDrawing)
            {
                if (isDrawing)
                {
                    return System.Windows.Input.Cursors.Pen;
                }
                else return System.Windows.Input.Cursors.Arrow;
            }

            return System.Windows.Input.Cursors.Arrow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
