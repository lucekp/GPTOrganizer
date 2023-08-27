using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GPTOrganizer
{
    internal class OpenAIApiClient
    {
        private static ILogger<OpenAIApiClient> logger = LoggerConfigurator.InitializeLogger<OpenAIApiClient>(LogLevel.Information);
        
        public static void ConfigureLogger(LogLevel logLevel)
        {
            logger = LoggerConfigurator.InitializeLogger<OpenAIApiClient>(logLevel);
        }

        public static async Task<Dictionary<string, string>> GetCategorySuggestions(List<string> fileNames,
            Program.Options options, int retryCount = 0)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = options.Timeout;
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");

                var categoriesString = string.Join(", ", Program.predefinedCategories); // Simplified to only predefinedCategories
                var filesString = string.Join(", ", fileNames);

                var contentMessage =
                    $"Given the file names {filesString}, which categories from the following list best fit each: {categoriesString}?";
                logger.LogInformation($"Constructing content message for OpenAI request.");
                logger.LogDebug($"Model specified in options: {options.Model}");
                var payload = new
                {
                    model = options.Model,
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = contentMessage
                        }
                    },
                    temperature = 0.7
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);

                HttpResponseMessage response;
                try
                {
                    response = await client.PostAsync((string?)options.Endpoint,
                        new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                    logger.LogInformation("Sent request to OpenAI.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception while sending request to OpenAI: {ex.Message}");
                    throw;
                }

                logger.LogDebug($"Received response with status code: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Error calling OpenAI API: {response.StatusCode} - {response.ReasonPhrase}");
                    return new Dictionary<string, string>();
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                logger.LogDebug($"Received response: {responseBody}");

                var suggestions = ExtractSuggestionsFromResponse(responseBody, fileNames);

                var missingFiles = fileNames.Except(suggestions.Keys).ToList();

                if (missingFiles.Any() && retryCount < options.MaxRetries)
                {
                    logger.LogWarning(
                        $"Missing categories for files. Retrying...");
                    var additionalSuggestions = await GetCategorySuggestions(missingFiles, options, retryCount + 1);
                    foreach (var entry in additionalSuggestions) suggestions[entry.Key] = entry.Value;
                }

                return suggestions;
            }
        }

        private static Dictionary<string, string> ExtractSuggestionsFromResponse(string jsonResponse,
            List<string> fileNames)
        {
            var parsedResponse = JObject.Parse(jsonResponse);
            var rawSuggestion = parsedResponse["choices"]?[0]?["message"]?["content"]?.ToString();

            var suggestions = new Dictionary<string, string>();

            var allCategories = Program.predefinedCategories; // Directly using predefinedCategories

            var firstCategoryIndex =
                allCategories.Select(cat => rawSuggestion.IndexOf(cat)).Where(index => index != -1).Min();
            var firstFileNameIndex = fileNames.Select(fn => rawSuggestion.IndexOf(fn)).Where(index => index != -1).Min();

            var isCategoryFirstFormat = firstCategoryIndex < firstFileNameIndex;

            if (isCategoryFirstFormat)
            {
                logger.LogDebug("Detected category-first format.");

                var blocks = rawSuggestion.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var block in blocks)
                {
                    var matchedCategory = allCategories.FirstOrDefault(cat => block.Contains(cat));

                    if (matchedCategory != null)
                    {
                        logger.LogDebug($"Identified category block.");

                        foreach (var fileName in fileNames)
                            if (block.Contains(fileName))
                            {
                                suggestions[fileName] = matchedCategory;
                            }
                    }
                    else
                    {
                        logger.LogWarning($"Unable to identify category for a block.");
                    }
                }
            }
            else
            {
                logger.LogDebug("Detected file-first format.");

                foreach (var fileName in fileNames)
                {
                    var startIndex = rawSuggestion.IndexOf(fileName);

                    if (startIndex == -1)
                    {
                        logger.LogWarning($"Filename not found in the response.");
                        continue;
                    }

                    var matchedCategory = allCategories
                        .Where(cat =>
                            rawSuggestion.IndexOf(cat, startIndex) > startIndex)
                        .OrderBy(cat => rawSuggestion.IndexOf(cat, startIndex))
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(matchedCategory))
                    {
                        suggestions[fileName] = matchedCategory;
                    }
                    else
                    {
                        suggestions[fileName] = "Miscellaneous";
                        logger.LogWarning(
                            $"No matched category found. Defaulting to Miscellaneous.");
                    }
                }
            }

            return suggestions;
        }
    }
}
