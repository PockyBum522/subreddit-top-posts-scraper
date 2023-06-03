﻿using Serilog;
using SubredditScraper.RawData;

namespace SubredditScraper.ThirdPartyWebsiteWorkers;

public class HttpDownloader
{
    private readonly ILogger _logger;
    private HttpClient? _httpClient;

    public HttpDownloader(ILogger logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task DownloadFile(string url, string fullPathToSaveTo)
    {
        if (!UrlHasWhitelistedExtension(url))
        {
            _logger.Error("Extension not whitelisted - URL: {FromUrl} | TO: {PostType}",
                url, fullPathToSaveTo);
        }

        if (_httpClient is null) throw new NullReferenceException();
        
        var response = await _httpClient.GetAsync(url);

        await using var fs = new FileStream(fullPathToSaveTo, FileMode.CreateNew);
        
        await response.Content.CopyToAsync(fs);
    }

    public async Task<string> GetWebsiteSourceHtml(string url)
    {
        // Otherwise, download the media:
        _httpClient ??= new HttpClient();
            
        var response = await _httpClient.GetAsync(url);

        var siteHtml = await response.Content.ReadAsStringAsync();

        return siteHtml;
    }
    
    public string GetFullPathToSaveTo(string trimmedUrl, string folderPathToSaveTo, int postNumber)
    {
        var fileNameOnly = Path.GetFileName(trimmedUrl)
            .Replace('?', '_');

        Directory.CreateDirectory(folderPathToSaveTo);

        return Path.Join(folderPathToSaveTo, $"TOP_{postNumber}_{fileNameOnly}");
    }
    
    private bool UrlHasWhitelistedExtension(string url)
    {
        var urlHasWhitelistedExtension = false;

        foreach (var extensionString in DirectDownloadExtensionsWhitelist.WhitelistedExtensions)
        {
            var lowerExtension = extensionString.ToLower();

            if (url.ToLowerInvariant().EndsWith(lowerExtension)) urlHasWhitelistedExtension = true;
        }

        return urlHasWhitelistedExtension;
    }
}