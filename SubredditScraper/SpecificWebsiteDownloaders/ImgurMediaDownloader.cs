using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using SubredditScraper.Interfaces;
using SubredditScraper.RawData;
using SubredditScraper.SpecificWebsiteDownloaders.HttpUtilities;

namespace SubredditScraper.SpecificWebsiteDownloaders;

public class ImgurMediaDownloader : IWebsiteMediaDownloader
{
    public string DomainToMatchOn => "imgur.com/a/";
    
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private readonly HttpDownloader _httpDownloader;

    public ImgurMediaDownloader(ILogger logger, HttpClient httpClient, HttpDownloader httpDownloader)
    {
        _logger = logger;
        _httpClient = httpClient;
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
            var albumHash = GetImgurAlbumHashFromFullAlbumUrl(url);

            var albumImagesJson = await GetAlbumAllImagesJsonAsync(albumHash);

            var albumImageUrls = GetImageUrlsFromJson(albumImagesJson);

            var imageNumberInAlbum = 0;

            foreach (var imageUrl in albumImageUrls)
            {
                var urlFileExtension = Path.GetExtension(imageUrl);
                
                imageNumberInAlbum++;

                var builtBaseFolderPath = Path.Combine(folderPathToSaveTo,
                    $"TOP_{postNumber.ToString("D3")}_ALBUM_{albumHash}");

                Directory.CreateDirectory(builtBaseFolderPath);
                
                var builtFullPath = Path.Join(builtBaseFolderPath,
                    $"ALBUM_{imageNumberInAlbum.ToString("D3")}{urlFileExtension}");

                await _httpDownloader.DownloadFile(imageUrl, builtFullPath);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.Warning("{Exception}", ex);
        }
    }

    private List<string> GetImageUrlsFromJson(string albumImagesJson)
    {
        var imageUrls = new List<string>();

        var returnJson = JsonConvert.DeserializeObject<dynamic>(albumImagesJson) ?? "error";

        // .data[0] = first image
        var imageDataArray = returnJson.data;
        var imageDataArrayCount = ((JArray)returnJson.data).Count;

        for (var i = 0; i < imageDataArrayCount; i++)
        {
            var currentImageUrl = (string)imageDataArray[i].link;
            
            if (!string.IsNullOrWhiteSpace(currentImageUrl))
                imageUrls.Add(currentImageUrl);
        }

        return imageUrls;
    }

    private async Task<string> GetAlbumAllImagesJsonAsync(string albumHash)
    {
        var returnUrls = new List<string>();
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {ImgurApiKeys.ImgurClientId}");

        var response = await _httpClient.GetAsync($"https://api.imgur.com/3/album/{albumHash}/images");

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
        
        var content = await response.Content.ReadAsStringAsync();
                
        return content;
    }

    private string GetImgurAlbumHashFromFullAlbumUrl(string url)
    {
        const string justBeforeAlbumHashNeedle = "/a/";
        
        var albumHashStart = url.IndexOf(justBeforeAlbumHashNeedle, StringComparison.InvariantCultureIgnoreCase);

        var imgurUrlWithBeforeAlbumHashRemoved = url.Substring(albumHashStart + justBeforeAlbumHashNeedle.Length);

        var forwardSlashLocation = imgurUrlWithBeforeAlbumHashRemoved.IndexOf('/', StringComparison.InvariantCultureIgnoreCase);

        if (forwardSlashLocation > 1)
        {
            var albumHash = imgurUrlWithBeforeAlbumHashRemoved.Substring(0, forwardSlashLocation);        
            
            return albumHash;
        }
        
        return imgurUrlWithBeforeAlbumHashRemoved;
    }
}