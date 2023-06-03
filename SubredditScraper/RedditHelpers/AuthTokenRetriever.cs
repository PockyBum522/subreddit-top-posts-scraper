using System.Diagnostics;
using Reddit.AuthTokenRetriever;
using SubredditScraper.RawData;

namespace SubredditScraper.RedditHelpers;

// TO USE THIS FILE, FILL IN THE LISTED AS NECESSARY ENTRIES IN ApiKeys.cs or rename ApiKeys_EXAMPLE.cs and its class
// to ApiKeys.cs and then fill it in.

// After that, just make the only call in Main() this:
// AuthTokenRetriever.GetAuthToken();

class AuthTokenRetriever
{
    // Change this to the path to your local web browser.  --Kris
    public const string BrowserPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";

    public static void GetAuthToken()
    {
        var appId = ApiKeys.RedditClientId;
        var appSecret = ApiKeys.RedditApiSecret;

        // Create a new instance of the auth token retrieval library.  --Kris
        var authTokenRetrieverLib = new AuthTokenRetrieverLib(appId, 8080, appSecret: appSecret);

        // Start the callback listener.  --Kris
        authTokenRetrieverLib.AwaitCallback();
        Console.Clear();  // Gets rid of that annoying logging exception message generated by the uHttpSharp library.  --Kris

        // Open the browser to the Reddit authentication page.  Once the user clicks "accept", Reddit will redirect the browser to localhost:8080, where AwaitCallback will take over.  --Kris
        OpenBrowser(authTokenRetrieverLib.AuthURL());

        Console.ReadKey();  // Hit any key to exit.  --Kris

        authTokenRetrieverLib.StopListening();

        Console.WriteLine("Token retrieval utility terminated.");
    }

    private static void OpenBrowser(string authUrl = "about:blank")
    {
        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(authUrl);
            Process.Start(processStartInfo);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // This typically occurs if the runtime doesn't know where your browser is.  Use BrowserPath for when this happens.  --Kris
            ProcessStartInfo processStartInfo = new ProcessStartInfo(BrowserPath)
            {
                Arguments = authUrl
            };
            Process.Start(processStartInfo);
        }
    }
}