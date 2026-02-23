using Microsoft.Extensions.DependencyInjection;
using NetOptimizer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NetOptimizer.Services
{
    public class WindowNavigator : IWindowNavigator
    {
        private readonly IServiceProvider _serviceProvider;

        public WindowNavigator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<TView>() where TView : Window
        {
            var newWindow = _serviceProvider.GetRequiredService<TView>();
            newWindow.Show();
        }

        public void ShowModalView<TView, TViewModel>()
         where TView : Window
         where TViewModel : class
        {

            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            var newWindow = _serviceProvider.GetRequiredService<TView>();
            newWindow.DataContext = viewModel;
            newWindow.ShowDialog();
        }
        public void ShowModalView<TView, TViewModel>(string message)
         where TView : Window
         where TViewModel : class, IModalWindow
        {

            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            var newWindow = _serviceProvider.GetRequiredService<TView>();
            newWindow.DataContext = viewModel;
            viewModel.Message = message;
            newWindow.ShowDialog();
        }
        public void ShowModalView<TView, TViewModel>(object model) where TView : Window
                                                                   where TViewModel : class
        {
            var viewModel = ActivatorUtilities.CreateInstance<TViewModel>(_serviceProvider, model);
            var newWindow = _serviceProvider.GetRequiredService<TView>();
            newWindow.DataContext = viewModel;
            newWindow.ShowDialog();
        }
        public void Close(object viewModel)
        {
            var window = Application.Current.Windows
                .Cast<Window>()
                .FirstOrDefault(w => w.DataContext == viewModel);

            if (window != null)
            {
                window?.Close();
            }
        }
        public void Minimize(object viewModel)
        {
            var window = Application.Current.Windows
                .Cast<Window>()
                .FirstOrDefault(w => w.DataContext == viewModel);

            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

    }
}
