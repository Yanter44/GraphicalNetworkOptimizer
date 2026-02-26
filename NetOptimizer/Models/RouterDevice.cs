using NetOptimizer.Enums;

namespace NetOptimizer.Models
{
    public class RouterDevice : Device
    {
        public RouterDevice(string name, int portCount, Action<RouterDevice> configure = null) : base(name, portCount)
        {
            this.Type = DeviceType.Router;
            configure?.Invoke(this);
        }
    }
}
