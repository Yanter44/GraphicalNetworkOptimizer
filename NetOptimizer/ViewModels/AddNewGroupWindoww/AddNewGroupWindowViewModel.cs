using NetOptimizer.Common;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Views.DopViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.AddNewGroupWindoww
{
    public class AddNewGroupWindowViewModel : INotifyPropertyChanged
    {
        public event Action RequestClose;
        public ICommand CloseCommand { get; }
        public ICommand ConfirmAndAddCommand { get; }
        private readonly IWindowNavigator _windowNavigator;

        private string _groupName;
        public string GroupName { get => _groupName; set { _groupName = value;  OnPropertyChanged(); } }
        public event Func<DeviceGroup, bool> GroupCreated;
        public AddNewGroupWindowViewModel(IWindowNavigator windowNavigator)
        {
            _windowNavigator = windowNavigator;
            CloseCommand = new RelayCommand(CloseWindow);
            ConfirmAndAddCommand = new RelayCommand(ConfirmAndAddGroup);
        }
        private void ConfirmAndAddGroup()
        {
            if (!string.IsNullOrWhiteSpace(GroupName))
            {
                var newgroupModel = new DeviceGroup()
                {
                    GroupName = GroupName,
                };
                bool result = GroupCreated.Invoke(newgroupModel);
                if (result)
                    CloseWindow();
                else
                    _windowNavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>("Группа с таким именем уже существует");
                  
            }
        }
        private void CloseWindow()
        {
            RequestClose?.Invoke();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
