using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NetOptimizer.Interfaces
{
    public interface IWindowNavigator
    {
        void NavigateTo<TView>() where TView : Window;
        void ShowModalView<TView, TViewModel>() where TView : Window where TViewModel : class;
        void ShowModalView<TView, TViewModel>(string message) where TView : Window where TViewModel : class, IModalWindow;
        void ShowModalView<TView, TViewModel>(object model) where TView : Window where TViewModel : class;
        void Close(object viewModel);
        void Minimize(object viewModel);
    }
}
