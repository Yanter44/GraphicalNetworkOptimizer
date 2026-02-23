using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NetOptimizer.Convertors
{
    public class DeviceTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DeviceType type)
            {
                string imageName = type switch
                {
                    DeviceType.PC => "pc.png",
                    DeviceType.Router => "router.png",
                    DeviceType.Switch => "switch.png",
                    DeviceType.Server => "server.png",
                    DeviceType.Printer => "printer.png",
                    DeviceType.IpVideoCam => "videocam.png",
                    DeviceType.AccessPoint => "accesspoint.png",
                    _ => "default.png"
                };

                return $"/Assets/Images/{imageName}";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
