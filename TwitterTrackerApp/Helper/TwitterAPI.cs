using System.Text;
using Newtonsoft.Json.Linq;
using DotNetEnv;

public class TwitterAPI
{


    public async Task<JObject> GetRetweetedOfTweetId(string tweetID, string accessToken)
    {
        var requestRetweet = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/2/tweets/" + tweetID + "/retweeted_by?user.fields=created_at"));
        requestRetweet.Headers.Add("Authorization", "Bearer " + accessToken);
        var httpClient = new HttpClient();


        HttpResponseMessage responseFromTwitter = await httpClient.SendAsync(requestRetweet);
        return JObject.Parse(await responseFromTwitter.Content.ReadAsStringAsync()); ;
    }

    public async Task<JObject> GetFollowerOfUserId(string UserId, string accessToken)
    {
        var requestToTwitter = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/2/users/" + UserId + "/followers?user.fields=created_at&expansions=pinned_tweet_id&tweet.fields=created_at"));
        requestToTwitter.Headers.Add("Authorization", "Bearer " + accessToken);
        var httpClient = new HttpClient();

        HttpResponseMessage responseFromTwitter = await httpClient.SendAsync(requestToTwitter);
        return JObject.Parse(await responseFromTwitter.Content.ReadAsStringAsync()); ;
    }


    public async Task<string> GetAccessToken()
    {
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token");
        var customerInfo = Convert.ToBase64String(new UTF8Encoding().GetBytes(Env.GetString("TWITTER_API_KEY") + ":" + Env.GetString("TWITTER_SCREET_KEY")));
        request.Headers.Add("Authorization", "Basic " + customerInfo);
        request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

        HttpResponseMessage response = await httpClient.SendAsync(request);

        string json = await response.Content.ReadAsStringAsync();
        dynamic item = JObject.Parse(json);
        return item["access_token"];
    }
}
