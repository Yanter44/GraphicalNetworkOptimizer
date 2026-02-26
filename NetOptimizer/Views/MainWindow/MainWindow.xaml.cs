using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using NetOptimizer.Models;
using NetOptimizer.ViewModels;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NetOptimizer.Interfaces;


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
            viewmodel.DeviceAdded += OnDeviceAddedToCanvas;
            viewmodel.ConnectionRequested += CreateVisualConnection;

            InitializeEditor(viewmodel);
        }
       

    }
}