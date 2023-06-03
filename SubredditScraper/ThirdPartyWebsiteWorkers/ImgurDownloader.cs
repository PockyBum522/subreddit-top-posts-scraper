using SubredditScraper.Interfaces;

namespace SubredditScraper.ThirdPartyWebsiteWorkers;

public class ImgurDownloader : IThirdPartyWebsiteDownloader
{
    public string DomainToMatchOn => "imgur.com";
    
    public Task GetMedia(string url, string folderPathToSaveTo, int postNumber)
    {
        throw new NotImplementedException();
    }
}