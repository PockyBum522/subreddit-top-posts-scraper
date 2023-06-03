using Serilog;

namespace SubredditScraper.Loggers;

public class LoggerBuilders
{
    public static string LogFolderPath => Path.Join(Program.BaseFolder, "Logs");
    
    public static ILogger GetApplicationLogger()
    {

        Directory.CreateDirectory(LogFolderPath);

        var logPath = Path.Join(LogFolderPath, "application.log");
        
        return
            new LoggerConfiguration()
                .Enrich.WithProperty("RedditScraperApplication", "RedditScraperSerilogContext")
                .MinimumLevel.Debug()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateLogger();
    }
    
    public static ILogger GetUnknownExtensionsLogger()
    {
        var logFolder = @"C:\Users\Public\Documents\Logs\RedditScraper\UnknownExtensionsLogs";

        Directory.CreateDirectory(logFolder);

        var logPath = Path.Join(LogFolderPath, "unknown_extensions.log");
        
        return
            new LoggerConfiguration()
                .Enrich.WithProperty("RedditScraperUnknownExtsApplication", "RedditScraperUnknownExtsSerilogContext")
                .MinimumLevel.Debug()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateLogger();
    }
}