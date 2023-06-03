using SubredditScraper.Loggers;
using SubredditScraper.RawData;
using SubredditScraper.RedditHelpers;
using SubredditScraper.ThirdPartyWebsiteWorkers;

namespace SubredditScraper;

public class Program
{
    private const string BaseFolder = @"C:\Users\Public\Documents\RedditScraper\";
    private const string DesktopFolder = @"D:\Dropbox\Documents\Desktop\";
    
    static Program()
    {
        var logger = LoggerBuilders.GetApplicationlogger();

        var httpClient = new HttpClient();
        
        var httpDownloader = new HttpDownloader(logger, httpClient);
        
        var redgifsDownloader = new RedgifsDownloader(logger, httpDownloader);
        var gfycatDownloader = new GfycatDownloader(logger, httpDownloader);
        
        var redditManager = new RedditManager(logger, httpDownloader, redgifsDownloader, gfycatDownloader);
    }
    
    public static async Task Main()
    {
        _redditManager.LogUsernameAndCakeDay();

        foreach (var subName in SubredditsToScrape.SubredditNames)
        {
            await _redditManager.ScrapeTopXOnSub(subName, 2000);
        }
    }
}