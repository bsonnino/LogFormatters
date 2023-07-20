using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", false, true)
    .Build();
                                
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConfiguration(configuration.GetSection("Logging"))
        .AddConsole()
        .AddConsoleFormatter<CsvLogFormatter, CsvFormatterOptions>();
});

var logger = loggerFactory.CreateLogger<Program>();
using(var scope1 = logger.BeginScope("Scope 1"))
{
    logger.LogInformation("This is an information message in scope 1");
    logger.LogWarning("This is a warning message in scope 1");
    logger.LogError("This is an error message in scope 1");
    logger.LogCritical("This is a critical message in scope 1");
}
using(var scope2 = logger.BeginScope("Scope 2"))
{
    logger.LogInformation("This is an information message in scope 2");
    logger.LogWarning("This is a warning message in scope 2");
    logger.LogError("This is an error message in scope 2");
    logger.LogCritical("This is a critical message in scope 2");
    using(var scope3 = logger.BeginScope("Scope 3"))
    {
        logger.LogInformation("This is an information message in scope 3");
        logger.LogWarning("This is a warning message in scope 3");
        logger.LogError("This is an error message in scope 3");
        logger.LogCritical("This is a critical message in scope 3");
    }
}
logger.LogTrace("This is a trace message");
logger.LogDebug("This is a debug message");
logger.LogInformation("This is an information message");
logger.LogWarning("This is a warning message");
logger.LogError("This is an error message");
logger.LogCritical("This is a critical message");
//Task.Delay(1000).Wait();


