using NetOptimizer.ViewModels;
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
    /// Логика взаимодействия для CreateDeviceWindow.xaml
    /// </summary>
    public partial class CreateDeviceWindow : Window
    {
        public CreateDeviceWindow()
        {
            InitializeComponent();
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
