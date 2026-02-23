using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetOptimizer.Interfaces
{
    public interface IYamlManager
    {
        Task<NetworkMap> CheckConfigAsync(string configText);
        Task<string> ReadYamlFileAsync(string pathToFile);
        Task<bool> CreateYamlFile(string path, string fileName, string yamltext);
    }
}
