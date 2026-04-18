
using NetOptimizer.ViewModels.DeviceParametrs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetOptimizer.Views
{
    /// <summary>
    /// Логика взаимодействия для DeviceParametrsWindow.xaml
    /// </summary>
    public partial class DeviceParametrsWindow : Window
    {
        public DeviceParametrsWindow()
        {
            InitializeComponent();
            this.Loaded += DeviceParametrsWindow_Loaded;
        }
        private void DeviceParametrsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DeviceParametrsViewModel vm)
            {
                vm.RequestClose += () => this.Close();
            }
        }
        private void NavBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                this.DragMove();
            }
        }
    }
}
