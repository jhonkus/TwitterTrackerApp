using Xunit;
using System;
using System.Threading.Tasks;
using System.Net;
namespace UnitTest;


public class TwitterAPITests
{

    [Fact]
    public async Task GET_GetAccessToken()
    {

        var twitterApi = new TwitterAPI();
        var response = await twitterApi.GetAccessToken();
        Console.WriteLine(response);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

}