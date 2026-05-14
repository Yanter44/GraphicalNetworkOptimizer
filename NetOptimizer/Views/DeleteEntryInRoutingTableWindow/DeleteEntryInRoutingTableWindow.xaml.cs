using NetOptimizer.ViewModels.AddRouteEntryToRoutingTableWindoww;
using NetOptimizer.ViewModels.DeleteEntryInRoutingTableWindoww;
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

namespace NetOptimizer.Views.DeleteEntryInRoutingTableWindow
{
    /// <summary>
    /// Логика взаимодействия для DeleteEntryInRoutingTableView.xaml
    /// </summary>
    public partial class DeleteEntryInRoutingTableWindow : Window
    {
        public DeleteEntryInRoutingTableWindow()
        {
            InitializeComponent();
            this.Loaded += DeleteEntryInRoutingTableWindow_Loaded;
        }

        private void DeleteEntryInRoutingTableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DeleteEntryInRoutingTableViewModel vm)
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
