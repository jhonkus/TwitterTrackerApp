using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TwitterTrackerApp.Models;
using TweetSharp;
using Newtonsoft.Json.Linq;
using DotNetEnv;
using System.Net;

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


            // access twitter API V2
            var twitterApi = new TwitterAPI();

            //get bearer token using OAuth2   
            var response = await twitterApi.GetAccessToken();
            if (response.StatusCode == HttpStatusCode.OK)
            {

                var item = JObject.Parse(await response.Content.ReadAsStringAsync());
                string bearerToken = item["access_token"]!.ToString();

                //check if user has retweet specific TWEET_ID
                var specificTweetId = Env.GetString("TWEETID");
                response = await twitterApi.GetRetweetedOfTweetId(specificTweetId, bearerToken);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseObj = JObject.Parse(await response.Content.ReadAsStringAsync());
                    var retweeters = responseObj!["data"]!.Children<JObject>();
                    var findUserInRetweeters = retweeters.FirstOrDefault(o => o["id"] != null && o["id"]!.ToString() == user?.IdStr);

                    //set user info for isReteet
                    userInfo.isRetweet = findUserInRetweeters != null ? true : false;
                    Console.WriteLine("==userRetweet:{0}", findUserInRetweeters);
                }

                //check if user login is follower of specific TWEETER ID
                var specificTwitterAccountId = Env.GetString("TWITTERID");
                response = await twitterApi.GetFollowerOfUserId(specificTwitterAccountId, bearerToken);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseObj = JObject.Parse(await response.Content.ReadAsStringAsync());
                    var followers = responseObj!["data"]!.Children<JObject>();
                    var findUserInFollowers = followers.FirstOrDefault(o => o["id"] != null && o["id"]!.ToString() == user?.IdStr);

                    //set user info for isFollower
                    userInfo.isFollower = findUserInFollowers != null ? true : false;
                    Console.WriteLine("==userFollower:{0}", findUserInFollowers);
                }
                
                ViewBag.SpecificTweetId = specificTweetId;
                ViewBag.SpecificTwitterAccountId = specificTwitterAccountId;
            }

            ViewBag.UserInfo = userInfo;


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
