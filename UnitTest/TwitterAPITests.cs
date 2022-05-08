using Xunit;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace UnitTest;


public class TwitterAPITests
{

    [Fact]
    public async Task RequestAPI()
    {
        var twitterApi = new TwitterAPI();
        var accessToken = "AAAAAAAAAAAAAAAAAAAAAFAncQEAAAAAONNY0mQwzUMjvoR01wZq42qdvQs%3DL0N3n7pf92refB45M2jqcZg8NeQjSn9pmy2gSBcq85EMutf4SA";
        var url = string.Format("https://api.twitter.com/2/users/{0}/followers?user.fields=created_at&expansions=pinned_tweet_id&tweet.fields=created_at", "55968218");
        var response = await twitterApi.RequestAPI(HttpMethod.Get, url, "Bearer " + accessToken);
        Console.WriteLine(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task GetFollowerOfUserId()
    {
        var accessToken = "AAAAAAAAAAAAAAAAAAAAAFAncQEAAAAAONNY0mQwzUMjvoR01wZq42qdvQs%3DL0N3n7pf92refB45M2jqcZg8NeQjSn9pmy2gSBcq85EMutf4SA";
        var specificTwitterAccountId = "55968218";

        var twitterApi = new TwitterAPI();
        var response = await twitterApi.GetFollowerOfUserId(specificTwitterAccountId, accessToken); 
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task GetRetweetedOfTweetId()
    {
        var accessToken = "AAAAAAAAAAAAAAAAAAAAAFAncQEAAAAAONNY0mQwzUMjvoR01wZq42qdvQs%3DL0N3n7pf92refB45M2jqcZg8NeQjSn9pmy2gSBcq85EMutf4SA";
        var tweetID = "1522892243807666177";

        var twitterApi = new TwitterAPI();
        var response = await twitterApi.GetRetweetedOfTweetId(tweetID, accessToken); 
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAccessToken()
    {

        var twitterApi = new TwitterAPI();
        var response = await twitterApi.GetAccessToken();
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

}