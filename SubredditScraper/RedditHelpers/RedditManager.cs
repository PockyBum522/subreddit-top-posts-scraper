using Reddit;
using Reddit.Controllers;
using Reddit.Inputs;
using Serilog;
using SubredditScraper.Interfaces;
using SubredditScraper.Loggers;
using SubredditScraper.RawData;
using SubredditScraper.ThirdPartyWebsiteWorkers;

namespace SubredditScraper.RedditHelpers;

public class RedditManager
{
    private HttpClient? _httpClient;
    
    private readonly ILogger _logger;
    
    private readonly RedditClient _redditClient;
    private readonly HttpDownloader _httpDownloader;
    private readonly List<IWebsiteMediaDownloader> _websiteContentFetchers;

    public RedditManager(ILogger logger, RedditClient redditClient, HttpDownloader httpDownloader, List<IWebsiteMediaDownloader> websiteContentFetchers)
    {
        _logger = logger;
        _redditClient = redditClient;
        _httpDownloader = httpDownloader;
        _websiteContentFetchers = websiteContentFetchers;
    }
    
    public void LogUsernameAndCakeDay()
    {
        _logger.Information("Username: " + _redditClient.Account.Me.Name);
        _logger.Information("Cake Day: " + _redditClient.Account.Me.Created.ToString("D"));
    }
    
    public async Task ScrapeTopXOnSub(string subredditName, int postsToScrape = 50)
    {
        if (subredditName.ToLower() == "announcements")
        {
            _logger.Warning("Subreddit name was 'announcements' and we are skipping that since it's probably in the list of subreddits accidentally");

            return;
        }
        
        var subToScrape = _redditClient.Subreddit(subredditName).About();

        var topPosts = subToScrape.Posts.GetTop(new TimedCatSrListingInput(t: "all", limit: postsToScrape));

        var subCounter = 0;
        
        foreach (var currentPost in topPosts)
        {
            await Task.Delay(1000);

            subCounter++;

            if (currentPost is LinkPost convertedCurrentPost)
            {
                await DownloadLinkedMedia(convertedCurrentPost, subCounter);
                
                _logger.Information("Now working [{Url}] Sub: {SubName} Post number: {PostNumber} ({PostTitle}) - Type: {PostType}",
                    convertedCurrentPost.URL,
                    subredditName,
                    subCounter,
                    currentPost.Title,
                    currentPost.GetType());
            }
            else
            {
                _logger.Error("No known post type when trying to download from: Sub: {SubName} Post number: {PostNumber} ({PostTitle}) - Type: {PostType}",
                    subredditName,
                    subCounter,
                    currentPost.Title,
                    currentPost.GetType());
            }
        }
    }

    private async Task DownloadLinkedMedia(LinkPost convertedCurrentPost, int postNumber)
    {
        if (string.IsNullOrWhiteSpace(convertedCurrentPost.URL) ||
            convertedCurrentPost.URL.StartsWith('/'))
        {
            _logger.Warning("No url on LinkPost when trying to download from: Sub {SubName} (TITLE: {PostTitle}) - Post type is {PostType}",
                convertedCurrentPost.Subreddit,
                convertedCurrentPost.Title,
                convertedCurrentPost.GetType());

            return;
        }

        try
        {
            // Otherwise, download the media:
            _httpClient ??= new HttpClient();
        
            var urlToDownload = convertedCurrentPost.URL;

            var fileNameOnly = Path.GetFileName(convertedCurrentPost.URL)
                .Replace('?', '_');

            var baseFolderWithSubredditName =
                Path.Join(Program.BaseFolder, "Downloads", convertedCurrentPost.Subreddit);

            Directory.CreateDirectory(baseFolderWithSubredditName);

            var newFullPath = Path.Join(baseFolderWithSubredditName, $"TOP_{postNumber}_{fileNameOnly}");

            foreach (var contentFetcher in _websiteContentFetchers)
            {
                var domainToMatch = contentFetcher.DomainToMatchOn.ToLower();
                
                if (!urlToDownload.ToLower().Contains(domainToMatch))
                {
                    continue;
                }
                
                //Otherwise:
                await contentFetcher.GetMedia(urlToDownload, baseFolderWithSubredditName, postNumber);
                
                return;
            }
            
            // If none of the downloaders matched and handled it, then download it directly
            await _httpDownloader.DownloadFile(urlToDownload, newFullPath);
        }
        catch (HttpRequestException ex)
        {
            _logger.Warning("{Exception}", ex);
        }
    }
}