using NetOptimizer.Models.AddDeviceSettingsModels;
using NetOptimizer.Models.Dtos;

namespace NetOptimizer.Services
{
    public class EventAggregator
    {
        public static EventAggregator Instance { get; } = new EventAggregator();

        public event Action<DeviceToAddDto, DeviceSettingsBase> DeviceCreated;

        public void PublishDeviceCreated(DeviceToAddDto dto, DeviceSettingsBase settings)
            => DeviceCreated?.Invoke(dto, settings);
    }
}
