using SubredditScraper.Interfaces;

namespace SubredditScraper.ThirdPartyWebsiteWorkers;

public class ImgurMediaDownloader : IWebsiteMediaDownloader
{
    public string DomainToMatchOn => "imgur.com";
    
    public Task GetMedia(string url, string folderPathToSaveTo, int postNumber)
    {
        throw new NotImplementedException();
    }
}