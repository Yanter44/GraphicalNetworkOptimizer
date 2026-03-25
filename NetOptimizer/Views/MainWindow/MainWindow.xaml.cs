using System.Windows;
using NetOptimizer.Interfaces;
using NetOptimizer.ViewModels.MainWindow;


namespace NetOptimizer.Views.MainWindow
{
    public partial class MainWindow : Window
    {
        private readonly IWindowNavigator _windowNavigator;
        public MainWindow(MainWindowViewModel viewmodel,  IWindowNavigator windowNavigator)
        {
            InitializeComponent();
            this.DataContext = viewmodel;
            _windowNavigator = windowNavigator;
            InitializeEditor(viewmodel);
        }
       

    }
}