using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace GPTOrganizer
{
    internal class CategoryManager
    {
        private static ILogger<CategoryManager> logger = LoggerConfigurator.InitializeLogger<CategoryManager>(LogLevel.Information);

        public static void ConfigureLogger(LogLevel logLevel)
        {
            logger = LoggerConfigurator.InitializeLogger<CategoryManager>(logLevel);
        }

        public static List<string> LoadCategoriesFromFile(string categoriesFilePath)
        {
            if (!File.Exists(categoriesFilePath))
            {
                logger.LogError($"Error: The Categories.json file at path '{categoriesFilePath}' does not exist.");
                throw new FileNotFoundException($"The Categories.json file at path '{categoriesFilePath}' does not exist.", categoriesFilePath);
            }

            logger.LogInformation($"Loading categories from file: '{categoriesFilePath}'");
            var jsonContent = File.ReadAllText(categoriesFilePath);
            return DeserializeCategories(jsonContent);
        }

        public static List<string> DeserializeCategories(string jsonContent)
        {
            try
            {
                var categories = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
                var flattenedCategories = GetFlattenedCategories(categories);

                logger.LogInformation($"Loaded and flattened {flattenedCategories.Count} categories.");

                return flattenedCategories;
            }
            catch (JsonException ex)
            {
                logger.LogError($"Error deserializing the Categories content: {ex.Message}");
                throw;  // rethrow the exception for the caller to handle
            }
        }

        private static List<string> GetFlattenedCategories(Dictionary<string, object> nestedCategories, string currentPath = "")
        {
            var result = new List<string>();

            foreach (var kvp in nestedCategories)
            {
                if (kvp.Value is JObject subCategoryObj)
                {
                    var newPath = string.IsNullOrEmpty(currentPath) ? kvp.Key : $"{currentPath}/{kvp.Key}";

                    // Debug log
                    logger.LogDebug($"Encountered a subcategory: {kvp.Key} at path: {newPath}");

                    var subCategories = subCategoryObj.ToObject<Dictionary<string, object>>();
                    result.AddRange(GetFlattenedCategories(subCategories, newPath));
                }
                else
                {
                    var fullPath = currentPath + "/" + kvp.Key;
                    result.Add(fullPath);

                    // Debug log
                    logger.LogDebug($"Added category to result: {fullPath}");
                }
            }

            return result;
        }

    }
}
