using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TwitterTrackerApp.Models;
using TweetSharp;
using Newtonsoft.Json.Linq;
using DotNetEnv;

namespace TwitterTrackerApp.Controllers;

public class HomeController : Controller
{


    private TwitterService service = new TwitterService(DotNetEnv.Env.GetString("TWITTER_API_KEY"), DotNetEnv.Env.GetString("TWITTER_SCREET_KEY"));
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
            OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);
            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);

            VerifyCredentialsOptions option = new VerifyCredentialsOptions();

            //Get user profile
            var user = service.VerifyCredentials(option);
            Console.WriteLine(" User: {0}", user?.Id);

            var userInfo = new UserInfoModel
            {
                Id = user?.IdStr,
                Name = user?.Name,
                Userpic = user?.ProfileImageUrl
            };


            var twitterApi = new TwitterAPI();

            // access twitter API V2
            //get bearer token using OAuth2   
            var bearerToken = await twitterApi.GetAccessToken();

            
            //check if user has retweet specific TWEET_ID
            var specificTweetId = Env.GetString("TWEETID");
            JObject responseObj = await twitterApi.GetRetweetedOfTweetId(specificTweetId, bearerToken);
            var userRetweet = responseObj!["data"]!.Children<JObject>().FirstOrDefault(o => o["id"] != null && o["id"]!.ToString() == user?.IdStr);

            //set user info for isReteet
            userInfo.isRetweet = userRetweet != null ? true : false;
            Console.WriteLine("==userRetweet:{0}", userRetweet);

            //check if user login is follower of specific TWEETER ID
            var specificTwitterAccountId = Env.GetString("TWITTERID");
            responseObj = await twitterApi.GetFollowerOfUserId(specificTwitterAccountId, bearerToken);
            var userFollower = responseObj!["data"]!.Children<JObject>().FirstOrDefault(o => o["id"] != null && o["id"]!.ToString() == user?.IdStr);

            //set user info for isFollower
            userInfo.isFollower = userFollower != null ? true : false;
            Console.WriteLine("==userFollower:{0}", userFollower);

            ViewBag.UserInfo = userInfo;
            ViewBag.SpecificTweetId = specificTweetId;
            ViewBag.SpecificTwitterAccountId = specificTwitterAccountId;

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
