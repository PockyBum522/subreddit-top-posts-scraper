namespace SubredditScraper.Interfaces;

public interface IWebsiteMediaDownloader
{
    public string DomainToMatchOn { get; }
    
    public Task GetMedia(string url, string folderPathToSaveTo, int postNumber);
}