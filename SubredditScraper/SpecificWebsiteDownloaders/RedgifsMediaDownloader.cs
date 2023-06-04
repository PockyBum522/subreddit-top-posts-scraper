using Serilog;
using SubredditScraper.Interfaces;
using SubredditScraper.SpecificWebsiteDownloaders.HttpUtilities;

namespace SubredditScraper.SpecificWebsiteDownloaders;

public class RedgifsMediaDownloader : IWebsiteMediaDownloader
{
    public string DomainToMatchOn => "redgifs.com";
    
    private readonly ILogger _logger;
    private readonly HttpDownloader _httpDownloader;

    public RedgifsMediaDownloader(ILogger logger, HttpDownloader httpDownloader)
    {
        _logger = logger;
        _httpDownloader = httpDownloader;
    }

    public async Task GetMedia(string url, string folderPathToSaveTo, int postNumber)
    {
        if (string.IsNullOrWhiteSpace(url) ||
            url.StartsWith('/'))
        {
            _logger.Warning("Url problem when trying to download from: {Url} to: {DestinationFolder}",
                url,
                folderPathToSaveTo);

            return;
        }

        try
        {
            var siteHtml = await _httpDownloader.GetWebsiteSourceHtml(url);

            var urlPrefixSearchForString = """property="og:video" content=""";
            
            var firstVideoDoubleQuoteIndex = siteHtml.IndexOf(urlPrefixSearchForString, 
                StringComparison.InvariantCultureIgnoreCase);

            if (firstVideoDoubleQuoteIndex < 1)
            {
                _logger.Error("Could not get valid starting string position for redgifs url out of: {Html}", siteHtml);
                _logger.Error("When trying to download from: {Url}", url);
                
                return;
            }

            firstVideoDoubleQuoteIndex += urlPrefixSearchForString.Length + 1;
            
            var trimmedHtml = siteHtml.Substring(firstVideoDoubleQuoteIndex);
            
            var secondVideoDoubleQuoteIndex = trimmedHtml.IndexOf("\"", 
                StringComparison.InvariantCultureIgnoreCase);

            var trimmedUrl = trimmedHtml.Substring(0, secondVideoDoubleQuoteIndex);
            
            if (!trimmedUrl.EndsWith(".mp4"))
            {
                _logger.Error("Could not get valid starting string position for redgifs url out of: {Html}", siteHtml);
                _logger.Error("When trying to download from: {Url}", url);
            }

            var fullPathToSaveTo = _httpDownloader.GetFullPathToSaveTo(trimmedUrl, folderPathToSaveTo, postNumber);
            
            _logger.Information("RedgifsDownloader: Saving {Url} | TO: {DestinationPath}", trimmedUrl, fullPathToSaveTo);
            
            await _httpDownloader.DownloadFile(trimmedUrl, fullPathToSaveTo);
        }
        catch (HttpRequestException ex)
        {
            _logger.Warning("{Exception}", ex);
        }
    }
}