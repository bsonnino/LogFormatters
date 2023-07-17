using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

public sealed class CsvFormatterOptions : ConsoleFormatterOptions
{
    public string? ListSeparator { get; set; }
}

public class CsvLogFormatter : ConsoleFormatter, IDisposable
{
    CsvFormatterOptions? _options;
    private readonly IDisposable? _optionsReloadToken;

    public CsvLogFormatter(IOptionsMonitor<CsvFormatterOptions> options) : base("CsvFormatter")
    {
        _options = options.CurrentValue;
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    private void ReloadLoggerOptions(CsvFormatterOptions currentValue)
    {
        _options = currentValue;
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        string? message =
            logEntry.Formatter?.Invoke(
                logEntry.State, logEntry.Exception);

        if (message is null)
        {
            return;
        }
        var scopeStr = "";
        if (_options?.IncludeScopes == true && scopeProvider != null)
        {
            var scopes = new List<string>();
            scopeProvider.ForEachScope((scope, state) => state.Add(scope?.ToString() ?? ""), scopes);
            scopeStr = string.Join("|", scopes);
        }
        var listSeparator = _options?.ListSeparator ?? ",";
        if (_options?.TimestampFormat != null)
        {
            var timestampFormat = _options.TimestampFormat;
            var timestamp = "\"" +DateTime.Now.ToLocalTime().ToString(timestampFormat, 
                CultureInfo.InvariantCulture) + "\"";
            textWriter.Write(timestamp);
            textWriter.Write(listSeparator);
        }
        var logMessage = $"\"{logEntry.LogLevel}\"{listSeparator}"+
            $"\"{logEntry.Category}[{logEntry.EventId}]\"{listSeparator}" +
            $"\"{scopeStr}\"{listSeparator}\"{message}\"";
        textWriter.WriteLine(logMessage);
    }

    public void Dispose() => _optionsReloadToken?.Dispose();
}
