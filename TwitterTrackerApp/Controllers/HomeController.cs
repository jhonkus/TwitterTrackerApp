using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TwitterTrackerApp.Models;
using TweetSharp;
using DotNetEnv;

namespace TwitterTrackerApp.Controllers;

public class HomeController : Controller
{

    //Read consumer key from .env
    private string TWITTER_API_KEY = DotNetEnv.Env.GetString("TWITTER_API_KEY");
    private string TWITTER_SECREET_KEY = DotNetEnv.Env.GetString("TWITTER_SCREET_KEY");

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

        Console.WriteLine(" TWITTER_API_KEY: {0}", TWITTER_API_KEY);
        TwitterService service = new TwitterService(TWITTER_API_KEY, TWITTER_SECREET_KEY);

        //Register this path to your Twitter App
        var redirectPath = $"{Request.Scheme}://{Request.Host.Value}/Home/ValidateTwitterAuth";
        Console.WriteLine(" redirectPathY: {0}", redirectPath);

        // Get token
        OAuthRequestToken requestToken = service.GetRequestToken(redirectPath);

        Uri uri = service.GetAuthenticationUrl(requestToken);

        return Redirect(uri.ToString());
    }

    public ActionResult ValidateTwitterAuth(string oauth_token, string oauth_verifier)
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
            TempData["Userpic"] = user?.ProfileImageUrl;

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
