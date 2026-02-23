using NetOptimizer.ViewModels;
using System.Windows;
using System.Windows.Input;


namespace NetOptimizer.Views.DopViews
{
    /// <summary>
    /// Логика взаимодействия для ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(ErrorWindowViewModel view)
        {
            InitializeComponent();
            this.DataContext = view;
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
