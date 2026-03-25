using NetOptimizer.ViewModels.CreateDeviceWindow;
using System.Windows;
using System.Windows.Input;

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
            this.Loaded += CreateDeviceWindow_Loaded;
        }
        private void CreateDeviceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CreateDeviceWindowViewModel vm)
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
