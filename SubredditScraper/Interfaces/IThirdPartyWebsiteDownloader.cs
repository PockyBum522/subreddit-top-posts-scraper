namespace SubredditScraper.Interfaces;

public interface IThirdPartyWebsiteDownloader
{
    public string DomainToMatchOn { get; }
    
    public Task GetMedia(string url, string folderPathToSaveTo, int postNumber);
}