using Newtonsoft.Json;
using Reddit;
using SubredditScraper.Interfaces;
using SubredditScraper.Loggers;
using SubredditScraper.RawData;
using SubredditScraper.RedditHelpers;
using SubredditScraper.ThirdPartyWebsiteWorkers;

namespace SubredditScraper;

public class Program
{
    internal const string BaseFolder = @"C:\Users\Public\Documents\RedditScraper\";
    internal const string JsonFileOfSubredditsToScrape = @"E:\Dropbox\Documents\Desktop\subredditsToScrape.json";
    
    private static readonly RedditManager RedditManager;

    static Program()
    {
        var logger = LoggerBuilders.GetApplicationLogger();
        var unknownExtensionsLogger = LoggerBuilders.GetUnknownExtensionsLogger();

        var httpClient = new HttpClient();
        
        var httpDownloader = new HttpDownloader(logger, unknownExtensionsLogger, httpClient);
        
        var redditClient = new RedditClient(appId: ApiKeys.RedditClientId, appSecret: ApiKeys.RedditApiSecret, refreshToken: ApiKeys.RefreshToken);

        var websiteContentFetchers = new List<IWebsiteMediaDownloader>();

        websiteContentFetchers.Add(new RedgifsMediaDownloader(logger, httpDownloader));
        websiteContentFetchers.Add( new GfycatMediaDownloader(logger, httpDownloader));
        
        RedditManager = new RedditManager(logger, redditClient, httpDownloader, websiteContentFetchers);
    }
    
    public static async Task Main()
    {
        var subredditsToScrape = await GetSubredditsToScrape();
             
         RedditManager.LogUsernameAndCakeDay();
        
         foreach (var subName in subredditsToScrape)
         {
             await RedditManager.ScrapeTopXOnSub(subName, 2000);
         }
    }

    private static async Task<List<string>> GetSubredditsToScrape()
    {
        var returnList = new List<string>();
        
        var subredditsToScrape = JsonConvert.DeserializeObject<string[]>(await File.ReadAllTextAsync(JsonFileOfSubredditsToScrape));

        if (subredditsToScrape is null) throw new NullReferenceException();
        if (subredditsToScrape.Length < 1) throw new NullReferenceException();

        for (var i = 0; i < subredditsToScrape.Length; i++)
            returnList.Add(subredditsToScrape[i]);
        
        return returnList;
    }
}