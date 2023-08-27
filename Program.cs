using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GPTOrganizer;

internal class Program
{
    public static ILogger<Program> logger;
    public static List<string> predefinedCategories;

    private static async Task Main(string[] args)
    {
        // Initial logger setup
        logger = LoggerConfigurator.InitializeLogger<Program>(LogLevel.Information);
        logger.LogInformation("Starting GPTOrganizer...");

        var parserResult = Parser.Default.ParseArguments<Options>(args);
        var tcs = new TaskCompletionSource<bool>();

        parserResult.WithParsed(async options =>
            {
                if (string.IsNullOrEmpty(options.ApiKey))
                {
                    logger.LogError("No OpenAI API Key provided and OPENAI_API_KEY environment variable is not set.");
                    tcs.SetResult(false);
                    return;
                }

                // Set Logger to user-defined level
                if (Enum.TryParse(options.LogLevel, true, out LogLevel logLevel))
                {
                    SetLoggerLevelForAll(logLevel);
                    logger.LogDebug($"Log level updated to: {logLevel}");
                }

                // Load categories
                try
                {
                    logger.LogDebug($"Loading categories from: {options.CategoriesFilePath}");
                    predefinedCategories = CategoryManager.LoadCategoriesFromFile(options.CategoriesFilePath);
                }
                catch (FileNotFoundException)
                {
                    logger.LogError(
                        $"Error: The Categories.json file at path '{options.CategoriesFilePath}' does not exist.");
                    tcs.SetResult(false);
                    return;
                }
                catch (JsonException ex)
                {
                    logger.LogError($"Error deserializing the Categories content: {ex.Message}");
                    tcs.SetResult(false);
                    return;
                }

                // Process files in directory
                logger.LogInformation($"Organizing files in directory: {options.DirectoryPath}");
                var files = Directory.GetFiles(options.DirectoryPath);
                var fileNames = files.Select(Path.GetFileName).ToList();
                logger.LogDebug($"Identified {fileNames.Count} files.");

                logger.LogInformation("Fetching category suggestions from OpenAI API...");
                var suggestions = await OpenAIApiClient.GetCategorySuggestions(fileNames, options);
                logger.LogInformation("Received suggestions from OpenAI API.");

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    if (suggestions.TryGetValue(fileName, out var category))
                        FileOrganizer.OrganizeFile(file, category, options.DirectoryPath, options.MoveFiles);
                    else
                        logger.LogWarning($"Missing category suggestion for: {fileName}");
                }

                logger.LogInformation("Finished organizing files.");
                tcs.SetResult(true);
            })
            .WithNotParsed(errors =>
            {
                if (!errors.IsHelp() && !errors.IsVersion())
                    logger.LogError("Failed to parse command-line arguments. Use -h for help.");
                tcs.SetResult(false);
            });


        await tcs.Task;
    }

    private static void SetLoggerLevelForAll(LogLevel logLevel)
    {
        logger = LoggerConfigurator.InitializeLogger<Program>(logLevel);
        CategoryManager.ConfigureLogger(logLevel);
        FileOrganizer.ConfigureLogger(logLevel);
        OpenAIApiClient.ConfigureLogger(logLevel);
    }


    public class Options
    {
        private string _apiKey;

        public Options()
        {
            CategoriesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Categories.json");
        }

        [Option('k', "api-key", Required = false,
            HelpText =
                "OpenAI API Key. If not provided, the program will attempt to use the OPENAI_API_KEY environment variable.")]
        public string ApiKey
        {
            get => _apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            set => _apiKey = value;
        }


        [Option('d', "directory", Required = true, HelpText = "Directory containing the files")]
        public string DirectoryPath { get; set; }

        [Option('c', "categories", Required = false,
            HelpText = "Path to the Categories.json file")]
        public string CategoriesFilePath { get; set; }

        [Option('m', "move", Required = false, Default = false,
            HelpText = "Move files instead of copying. Default is false (copy).")]
        public bool MoveFiles { get; set; }

        [Option('l', "log-level", Required = false, Default = "Information",
            HelpText =
                "Set the minimum log level (Trace, Debug, Information, Warning, Error, Critical). Default is Information.")]
        public string LogLevel { get; set; }

        [Option('t', "timeout", Required = false, Default = 300,
            HelpText = "Timeout for the API call in seconds. Default is 300 seconds (5 minutes).")]
        private int ApiTimeoutSeconds { get; } = 300; // Default of 5 minutes

        public TimeSpan Timeout => TimeSpan.FromSeconds(ApiTimeoutSeconds);

        [Option('r', "retries", Required = false, Default = 5,
            HelpText = "Max number of retries for the API call. Default is 5.")]
        public int MaxRetries { get; set; } = 5;

        [Option('e', "endpoint", Required = false, Default = "https://api.openai.com/v1/chat/completions",
            HelpText = "Specify the OpenAI API endpoint.")]
        public string Endpoint { get; set; }

        [Option('o', "model", Required = false, Default = "gpt-3.5-turbo",
            HelpText = "Specify the OpenAI model to be used. Options include: " +
                       "gpt-3.5-turbo, " +
                       "gpt-3.5-turbo-0301, " +
                       "gpt-3.5-turbo-0613, " +
                       "gpt-3.5-turbo-16k, " +
                       "gpt-3.5-turbo-16k-0613, " +
                       "gpt-4, " +
                       "gpt-4-0314, " +
                       "gpt-4-0613.")]
        public string Model { get; set; }

        [Option('h', "help", HelpText = "Display this help message")]
        public bool DisplayHelp { get; set; }
    }
}