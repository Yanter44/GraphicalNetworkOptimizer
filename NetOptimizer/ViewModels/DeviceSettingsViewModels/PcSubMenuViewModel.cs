using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.ViewModels.DeviceSettingss
{
    public class PcSubMenuViewModel : INotifyPropertyChanged
    {
        private bool _includeInBudget;
        public bool IncludeInBudget { get => _includeInBudget; set { _includeInBudget = value; OnPropertyChanged(); } }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
