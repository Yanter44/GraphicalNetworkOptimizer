using NetOptimizer.Enums;

namespace NetOptimizer.Models
{
    public class SwitchDevice : Device
    {
        public SwitchDevice(string name, int portCount, Action<SwitchDevice> configure = null) : base(name, portCount)
        {
            this.Type = DeviceType.Switch;
            configure?.Invoke(this);
        }
    }
}
