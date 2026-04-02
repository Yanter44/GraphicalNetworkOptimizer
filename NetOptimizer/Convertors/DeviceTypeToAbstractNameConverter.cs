using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NetOptimizer.Convertors
{
    public class DeviceTypeToAbstractNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                if (value is DeviceType type)
                {
                    string deviceName = type switch
                    {
                        DeviceType.PC => "Пк",
                        DeviceType.Router => "Маршрутизаторы",
                        DeviceType.Switch => "Коммутаторы",
                        DeviceType.Server => "Серверы",
                        DeviceType.Printer => "Принтеры",
                        DeviceType.IpVideoCam => "Видеокамеры",
                        DeviceType.AccessPoint => "Точки доступа",
                        _ => "Неизвестное устройство"
                    };

                    return $"{deviceName}";
                }
            }
          return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
