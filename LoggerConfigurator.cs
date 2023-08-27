using Microsoft.Extensions.Logging;

namespace GPTOrganizer;

internal class LoggerConfigurator
{
    public static ILogger<T> InitializeLogger<T>(LogLevel logLevel)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(logLevel)
                .AddConsole();
        });
        return loggerFactory.CreateLogger<T>();
    }
}
