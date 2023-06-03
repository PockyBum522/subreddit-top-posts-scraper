namespace SubredditScraper.RawData;

public class ApiKeys
{
	// To get these, go to: https://www.reddit.com/prefs/apps/ and click "Create another app" or "Create new app"
	
	// When it asks you for type, between web app/installed app/script, choose script.
	
	// You don't need to fill in an "about uri"
	
	// For redirect URI, put EXACTLY: http://localhost:8080/Reddit.NET/oauthRedirect
	
	// This is what you typed in for "name" and is what shows up in blue on the prefs/apps page
    internal static string RedditAppId => "subbred_scraper_01";
    
	// This shows up a few lines under the name. It should show up right below the script type. For me, this is the line immediately below "Personal use script"
    internal static string RedditClientId => "swbL-F7r7oKc91T1Wsc5Qw";
    
	// This is what shows under the "secret" section for the application. You have to hit "Edit" to see this.
    internal static string RedditApiSecret => "48B3oARCTFX7dKSD5nZOU4qz5symVw";

	// This must be gotten by authorizing your application using AuthTokenRetriever.cs
    internal static string AccessToken => "eyJhbGciOiJSUzI1NiIsImtpZCI6IlNIQTI1NjpzS3dsMnlsV0VtMjVmcXhwTU40cWY4MXE2OWFFdWFyMnpLMUdhVGxjdWNZIiwidHlwIjoiSldUIn0.eyJzdWIiOiJ1c2VyIiwiZXhwIjoxNjg1ODI5NjI0LjAyOTcwNCwiaWF0IjoxNjg1NzQzMjI0LjAyOTcwMywianRpIjoieWk5eWtDRk41cFRMb2lMSy04T0RKV0VpaXd1YWd3IiwiY2lkIjoic3diTC1GN3I3b0tjOTFUMVdzYzVRdyIsImxpZCI6InQyX25yMmh3IiwiYWlkIjoidDJfbnIyaHciLCJsY2EiOjE0MzI3NTg3OTQ3NTcsInNjcCI6ImVKeEVqa0Z1eFRBSVJPX0NPamVxdXNBRzU2UGFJUUtjS3JldnFQUGIzV2dHWnQ0SFZHTWlDWWNOaGxMVkkwektETFhIR1NqOUwydXl3d1ktaTFlVHdxbkRabzFwVEI1MzUzeTZOREw1bGk5aGtzam4yMmY1M19GWnh2S1Z1dTVMbk9wdmlOWlJMT193NHVWb3ZQZ1h5QmdKTmpoTkxnd2U3STQ3ci1CVXkwNGhQa0xpaGcyNlhEend3RDFic0ZhZHg3TWFocTFKZlNpZjBqZXNVcHBMT2ZjR0c3ekVReTA3Rjl2blR3QUFBUF9fT1NCc2h3IiwicmNpZCI6IkZHNHpibnJVX1J6SWdNYnh2WVRGckMyVFhGdzRMc1dveXh2RFNGYlZtZmsiLCJmbG8iOjh9.mC0JkO4WJOogU_n6l4Ko0JLSQQjJEVJNQNxHaRn4itdN2etjl4ugIKbfFhaCvXgsrC6r_aawzgSkS6sDiLsDAN1QGoEwW9V4LZCkHg-CW-ICEst-fzYtoqUGrnnq62ZIdJdp8R2LqYj74y3OqTcNUZKeTS_bF1Gmty_rV6M64EWOt5t05ltSUeqTYqH-zCqxK2pd9bYfZI0qjKy-Q8aTVnLY4ufSyw8psXtuJqlNkTjDgAsX3bh3n4C7qBUNLlJFaOp1M49Cdz-d_XQ_74gUc1YHMY6Uy3decruZbtPtwAe6Y8-mqLUsyyk5zdNA66NTQcqm8CeEymLKWogythSePw";
    
	// This must be gotten by authorizing your application using AuthTokenRetriever.cs
    internal static string RefreshToken => "39894116-f9HD9_rBtUFuLcul949FbnhcYKQMgg";
}