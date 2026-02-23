using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.Dtos
{
    public class DeviceToAddDto
    {
        public string DeviceName { get; set; }
        public DeviceType Type { get; set; }
        public string ImagePath => Type switch
        {
            DeviceType.Router => "Assets/Images/router.png",
            DeviceType.Switch => "Assets/Images/switch.png",
            DeviceType.PC => "Assets/Images/pc.png",
            DeviceType.IpVideoCam => "Assets/Images/videocam.png",
            DeviceType.Server => "Assets/Images/server.png",
            DeviceType.AccessPoint => "Assets/Images/accesspoint.png",
            _ => "Assets/Images/delete.png"
        };
    }
}
