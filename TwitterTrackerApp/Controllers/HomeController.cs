using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TwitterTrackerApp.Models;
using TweetSharp;
using Newtonsoft.Json.Linq;

namespace TwitterTrackerApp.Controllers;

public class HomeController : Controller
{

    //Read consumer key from .env
    private string TWITTER_API_KEY = DotNetEnv.Env.GetString("TWITTER_API_KEY");
    private string TWITTER_SECREET_KEY = DotNetEnv.Env.GetString("TWITTER_SCREET_KEY");
    private string SPECIFIC_TWEETID = DotNetEnv.Env.GetString("TWEETID");
    private string SPECIFICT_TWEETERID = DotNetEnv.Env.GetString("TWEETERID");

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


    [HttpGet]
    public ActionResult TwitterAuth()
    {

        TwitterService service = new TwitterService(TWITTER_API_KEY, TWITTER_SECREET_KEY);

        //Register this path to your Twitter App
        var redirectPath = $"{Request.Scheme}://{Request.Host.Value}/Home/ValidateTwitterAuth";

        // Get token
        OAuthRequestToken requestToken = service.GetRequestToken(redirectPath);

        Uri uri = service.GetAuthenticationUrl(requestToken);

        return Redirect(uri.ToString());
    }

    public async Task<ActionResult> ValidateTwitterAuth(string oauth_token, string oauth_verifier)
    {
        var requestToken = new OAuthRequestToken(oauth_token);
        try
        {
            TwitterService service = new TwitterService(TWITTER_API_KEY, TWITTER_SECREET_KEY);
            OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);
            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);

            VerifyCredentialsOptions option = new VerifyCredentialsOptions();

            //Get user profile
            TwitterUser? user = service.VerifyCredentials(option);
            Console.WriteLine(" User: {0}", user?.Id);
            TempData["Id"] = user?.Id;
            TempData["Name"] = user?.Name;
            TempData["ScreenName"] = user?.ScreenName;
            TempData["Userpic"] = user?.ProfileImageUrl;


            var twitterApi = new TwitterAPI();

            // access twitter API V2

            //get bearer token    
            var bearerToken = await twitterApi.GetAccessToken();

            //check if user has retweet specific TWEET_ID
            var responseObj = await twitterApi.GetRetweetedOfTweetId(SPECIFIC_TWEETID, bearerToken);
            var retweeters = (JArray)responseObj["data"];
            JObject userRetweet = retweeters.Children<JObject>().FirstOrDefault(o => o["id"] != null && o["id"].ToString() == user.Id.ToString());
            TempData["isRetweet"] = userRetweet != null ? "yes" : "no";
            Console.WriteLine("==userRetweet:{0}", userRetweet);

            //check if user login is follower of specific TWEETER ID
            var respObj2 = await twitterApi.GetFollowerOfUserId(SPECIFICT_TWEETERID, bearerToken);
            var followers = (JArray)respObj2["data"];
            JObject userFollower = followers.Children<JObject>().FirstOrDefault(o => o["id"] != null && o["id"].ToString() == user.Id.ToString());
            TempData["isFollower"] = userFollower != null ? "yes" : "no";

            Console.WriteLine("==userFollower:{0}", userFollower);

            return View();
        }
        catch
        {
            throw;
        }

    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
