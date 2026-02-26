using NetOptimizer.Models;

namespace NetOptimizer.Interfaces
{
    public interface IYamlManager
    {
        Task<NetworkMap> CheckConfigAsync(string configText);
        Task<string> ReadYamlFileAsync(string pathToFile);
        Task<bool> CreateYamlFile(string path, string fileName, string yamltext);
    }
}
