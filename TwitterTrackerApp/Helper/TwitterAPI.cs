using System.Text;
using DotNetEnv;

public class TwitterAPI
{

    public async Task<HttpResponseMessage> GetRetweetedOfTweetId(string tweetID, string accessToken)
    {
        var url = string.Format("https://api.twitter.com/2/tweets/{0}/retweeted_by?user.fields=created_at", tweetID);
        var response = await RequestAPI(HttpMethod.Get, url, "Bearer " + accessToken);
        return response;
    }


    public async Task<HttpResponseMessage> GetFollowerOfUserId(string UserId, string accessToken)
    {
        var url = string.Format("https://api.twitter.com/2/users/{0}/followers?user.fields=created_at&expansions=pinned_tweet_id&tweet.fields=created_at", UserId);
        var response = await RequestAPI(HttpMethod.Get, url, "Bearer " + accessToken);
        return response;
    }

    public async Task<HttpResponseMessage> GetAccessToken()
    {
        var url = "https://api.twitter.com/oauth2/token";
        var customerInfo = Convert.ToBase64String(new UTF8Encoding().GetBytes(Env.GetString("TWITTER_API_KEY") + ":" + Env.GetString("TWITTER_SCREET_KEY")));
        var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await RequestAPI(HttpMethod.Post, url, "Basic " + customerInfo, content);
        return response;
    }



    public async Task<HttpResponseMessage> RequestAPI(HttpMethod metod, string apiUrl, string authentication, StringContent? content = null)
    {
        var request = new HttpRequestMessage(metod, string.Format(apiUrl));
        request.Headers.Add("Authorization", authentication);
        if (content is not null)
        {
            request.Content = content;
        }
        var httpClient = new HttpClient();
        var response = await httpClient.SendAsync(request);
        return response;
    }

}
