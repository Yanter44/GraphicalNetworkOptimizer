using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetOptimizer.Enums;
using NetOptimizer.Interfaces;
using NetOptimizer.Services;
using NetOptimizer.ViewModels;
using NetOptimizer.ViewModels.AddNewGroupWindoww;
using NetOptimizer.ViewModels.ConnectToGroupWindoww;
using NetOptimizer.ViewModels.MainWindow;
using NetOptimizer.ViewModels.MainWindoww;
using NetOptimizer.Views;
using NetOptimizer.Views.DopViews;
using NetOptimizer.Views.MainWindow;
using System.Windows;
namespace NetOptimizer
{
    public partial class App : Application
    {
        private readonly IHost _host;
        public App()
        {
            _host = Host.CreateDefaultBuilder()
             .ConfigureServices((context, services) =>
             {
                 services.AddHttpClient(ApiServers.NetOptimizerApi.ToString(), c =>
                 {
                     c.BaseAddress = new Uri("https://localhost:7244/");
                 });
                 services.AddSingleton<INetOptimizerApiService, NetOptimizerApiService>();
                 services.AddSingleton<DeviceCatalogService>();
                 services.AddSingleton<IWindowNavigator, WindowNavigator>();
                 services.AddTransient<IYamlManager, YamlNetworkManager>();
                 services.AddTransient<IFileService, FileService>();

                 services.AddTransient<MainWindow>();
                 services.AddTransient<CreateDeviceWindow>();
                 services.AddTransient<ErrorWindow>();
                 services.AddTransient<InfoWindow>();
                 services.AddTransient<AddNewGroupWindow>();
                 services.AddTransient<ConnectToGroupWindow>();

                 services.AddTransient<AddNewGroupWindowViewModel>();
                 services.AddTransient<ConnectToGroupWindowViewModel>();
                 services.AddTransient<EditorViewModel>();
                 services.AddTransient<SandboxViewModel>();
                 services.AddTransient<CanvasViewModel>();
                 services.AddTransient<MainWindowViewModel>();
                 services.AddTransient<ErrorWindowViewModel>();
                 services.AddTransient<InfoWindowViewModel>();
             }).Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                var catalogStore = _host.Services.GetRequiredService<DeviceCatalogService>();
                await catalogStore.LoadCatalogAsync();
                var main = _host.Services.GetRequiredService<MainWindow>();
                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("API Offline: " + ex.Message);
                Shutdown();
            }

       
        }
    }

}
