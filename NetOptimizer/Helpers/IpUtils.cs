using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Helpers
{
    public static class IpUtils
    {
        public static bool IsSameSubnet(string ip1, string ip2, string mask)
        {
            var a = ToInt(ip1);
            var b = ToInt(ip2);
            var m = ToInt(mask);

            return (a & m) == (b & m);
        }

        private static uint ToInt(string ip)
        {
            var parts = ip.Split('.').Select(byte.Parse).ToArray();
            return ((uint)parts[0] << 24)
                 | ((uint)parts[1] << 16)
                 | ((uint)parts[2] << 8)
                 | parts[3];
        }
        public static bool IsValidIp(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            var parts = ip.Split('.');

            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out var value))
                    return false;

                if (value < 0 || value > 255)
                    return false;
            }
            return true;
        }
    }
}
