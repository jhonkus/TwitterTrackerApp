# TwitterTrackerApp
Using Twitter API to show if user login follow specific twitter account and retweet specifict tweet id

this project use .NET core MVC.


Objective of this project

- Button to connect to Twitter using OAuth2
- Use the OAuth to get an access token to access the twitter API on behalf of the user
- The authenticated user has followed a specific twitter account (Twitter account user ID configurable in the app settings)
- The authenticated user has retweeted a specific tweet (Tweet ID configurable in the app settings)
- Display if the user has followed and retweeted
- unit tests for the API methods


Package used: TweetSharp-Unofficial-DotNetStandard, newtonseof, DotNetEnv


Please change value in .env file with yours.

Note: your Twitter Dev Account should be Elevated?

How to install:

This project use dotnet Core 6.0 SDK, please install it first to your computer.

```
git clone https://github.com/jhonkus/TwitterTrackerApp.git
cd TwitterTrackerApp
cd TwitterTrackerApp
dotnet restore
dotnet build
dotnet run

```