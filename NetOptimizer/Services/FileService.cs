using Microsoft.Win32;
using NetOptimizer.Interfaces;

namespace NetOptimizer.Services
{
    public class FileService : IFileService
    {
        public string OpenFileDialog(string filter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = filter
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }
    }
}
