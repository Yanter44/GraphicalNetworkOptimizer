using NetOptimizer.ViewModels.AddNewGroupWindoww;
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
    /// Логика взаимодействия для AddNewGroupWindow.xaml
    /// </summary>
    public partial class AddNewGroupWindow : Window
    {
        public AddNewGroupWindow()
        {
            InitializeComponent();
            this.Loaded += AddNewGroupWindow_Loaded;
        }

        private void AddNewGroupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddNewGroupWindowViewModel vm)
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
