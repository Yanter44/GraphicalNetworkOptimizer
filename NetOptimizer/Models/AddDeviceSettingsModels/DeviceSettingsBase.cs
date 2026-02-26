using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetOptimizer.Models.AddDeviceSettingsModels
{
    public abstract class DeviceSettingsBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public IEnumerable<PropertyInfo> GetEditableProperties()
        {
            return this.GetType()
                       .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.CanRead && p.CanWrite);
        }
    }
}
