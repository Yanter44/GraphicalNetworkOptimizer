using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using NetOptimizer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text;
using System.Windows;

namespace NetOptimizer.Views.MainWindow
{
    public partial class MainWindow
    {
        private void InitializeEditor(MainWindowViewModel vm)
        {
            LoadCustomYamlHighlighting();
            this.Loaded += (s, e) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    YamlEditor.Text = vm.CurrentYamlText;
                    YamlEditor.TextChanged += (sender, args) =>
                    {
                        vm.CurrentYamlText = YamlEditor.Text;
                    };
                    vm.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == nameof(vm.CurrentYamlText))
                        {
                            if (YamlEditor.Text != vm.CurrentYamlText)
                                YamlEditor.Text = vm.CurrentYamlText;
                        }
                    };
                }
            };
        }

        private void LoadCustomYamlHighlighting()
        {
            var assembly = Assembly.GetExecutingAssembly();

            string resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith("MyYmlSyntax.xshd"));

            if (string.IsNullOrEmpty(resourceName))
            {
                MessageBox.Show("ОШИБКА: Файл .xshd не найден в ресурсах сборки!");
                foreach (var name in assembly.GetManifestResourceNames())
                    MessageBox.Show($"Доступный ресурс: {name}");
                return;
            }

            using (Stream s = assembly.GetManifestResourceStream(resourceName))
            {
                if (s == null) return;
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    YamlEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}
