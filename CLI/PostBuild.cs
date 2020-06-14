using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PointsBot.CLI
{
    public class PostBuild
    {
        private const string DotNetVersion = "netcoreapp3.1";

        private static DirectoryInfo SourceDirectory(string environmentName) =>
            new DirectoryInfo($"G:\\Projects\\PointsBot\\CLI\\bin\\{environmentName}\\{DotNetVersion}");

        private static DirectoryInfo DestinationDirectory(string environmentName) =>
            new DirectoryInfo($"G:\\PointsBotCli\\{environmentName}\\{DotNetVersion}");

        private static readonly DirectoryInfo ProdDirectory = new DirectoryInfo("G:\\PointsBotCli\\prod");

        public static void Execute(string environment)
        {
            CopyAll(SourceDirectory(environment), DestinationDirectory(environment));
            CopyAll(DestinationDirectory(environment), ProdDirectory);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (var file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (var subDirectoryInSource in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(subDirectoryInSource.Name);
                CopyAll(subDirectoryInSource, nextTargetSubDir);
            }
        }
    }
}
