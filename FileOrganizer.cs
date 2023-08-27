using Microsoft.Extensions.Logging;
using System.IO;

namespace GPTOrganizer
{
    internal class FileOrganizer
    {
        private static ILogger<FileOrganizer> logger = LoggerConfigurator.InitializeLogger<FileOrganizer>(LogLevel.Information);

        public static void ConfigureLogger(LogLevel logLevel)
        {
            logger = LoggerConfigurator.InitializeLogger<FileOrganizer>(logLevel);
        }

        public static void OrganizeFile(string filePath, string category, string directoryPath, bool moveFiles)
        {
            string fileName = Path.GetFileName(filePath);
            logger.LogInformation($"Organizing file: {fileName} under category: {category}");

            var categories = category.Split('/');
            var currentPath = Path.Combine(directoryPath, "GPTOrganizer");

            foreach (var cat in categories)
            {
                var safeCategory = MakeSafeFolderName(cat);
                currentPath = Path.Combine(currentPath, safeCategory);
                if (!Directory.Exists(currentPath))
                {
                    logger.LogInformation($"Creating directory: {currentPath}");
                    Directory.CreateDirectory(currentPath);
                }
            }

            var destinationPath = Path.Combine(currentPath, fileName);
            if (moveFiles)
            {
                if (File.Exists(destinationPath))
                    logger.LogWarning($"File {fileName} already exists at destination. It will be overwritten.");
                File.Move(filePath, destinationPath);
                logger.LogInformation($"Moved {fileName} to: {destinationPath}");
            }
            else
            {
                File.Copy(filePath, destinationPath, true);
                logger.LogInformation($"Copied {fileName} to: {destinationPath}");
            }
        }

        private static string MakeSafeFolderName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }
    }
}
