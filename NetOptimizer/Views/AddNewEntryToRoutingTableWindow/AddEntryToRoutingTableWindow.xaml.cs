using NetOptimizer.ViewModels.AddRouteEntryToRoutingTableWindoww;
using NetOptimizer.ViewModels.CreateVlanOnDeviceWindoww;
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

namespace NetOptimizer.Views.AddNewEntryToRoutingTableWindow
{
    /// <summary>
    /// Логика взаимодействия для AddEntryToRoutingTableWindow.xaml
    /// </summary>
    public partial class AddEntryToRoutingTableWindow : Window
    {
        public AddEntryToRoutingTableWindow()
        {
            InitializeComponent();
            this.Loaded += AddEntryToRoutingTableWindow_Loaded;
        }

        private void AddEntryToRoutingTableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddEntryToRoutingTableViewModel vm)
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
