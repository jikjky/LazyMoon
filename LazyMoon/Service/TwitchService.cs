using Google.Apis.Auth.OAuth2;
using LazyMoon.Class;
using LazyMoon.Model;
using LazyMoon.Model.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;


namespace LazyMoon.Service
{
    public class TwitchService
    {
        public string OAuth { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public void SetOAuth(string _OAuth, string _ClientId, string _ClientSecret)
        {
            OAuth = _OAuth;
            ClientId = _ClientId;
            ClientSecret = _ClientSecret;
        }

        private readonly DBUserService _dbUserService;
        private readonly TwitchAPI _twitchAPI;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;


        private async Task<string> GetAccessTokken(string ClientId, string ClientSecret)
        {
            string tokenUrl = "https://id.twitch.tv/oauth2/token";
            string postData = $"client_id={ClientId}&client_secret={ClientSecret}&grant_type=client_credentials";

            using var httpClient = _httpClientFactory.CreateClient();
            var content = new StringContent(postData);
            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");

            HttpResponseMessage response = httpClient.PostAsync(tokenUrl, content).Result;
            string responseContent = await response.Content.ReadAsStringAsync();

            // Parse the response content to extract the access token
            // Note: For proper JSON parsing, you should consider using a JSON library.
            string[] parameters = responseContent.Split(',');
            string[] value = parameters[0].Split(':');
            string accessToken = value[1].Replace("\"", "").Trim();

            return accessToken;
        }

        public TwitchService(TwitchAPI twitchAPI, DBUserService dbUserService, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _dbUserService = dbUserService;
            _twitchAPI = twitchAPI;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;

            string oAuth = _configuration.GetValue<string>("TwitchOAuth:OAuth");
            string clientId = _configuration.GetValue<string>("TwitchOAuth:ClientId");
            string clientSecret = _configuration.GetValue<string>("TwitchOAuth:ClientSecret");
            _twitchAPI.Settings.ClientId = clientId;
            _twitchAPI.Settings.AccessToken = GetAccessTokken(clientId, clientSecret).Result;
            _twitchAPI.Settings.Secret = clientSecret;
            SetOAuth(oAuth, clientId, clientSecret);
        }

#nullable enable
        public async Task<User?> LoginAsync(string authToken)
        {
            var users = await _twitchAPI.Helix.Users.GetUsersAsync(accessToken: authToken);
            var user = users.Users.FirstOrDefault();
            if (user == null)
                return null;
            var dbUser = await _dbUserService.GetUserOrNullAsync(user.Login ?? "");
            if (dbUser == null)
            {
                var addObject = new UserDTO() { Key = authToken, UserId = user.Id, Name = user.Login };
                return await _dbUserService.SetUserOrNullAsync(addObject);
            }
            return dbUser;
        }

        public async Task<bool?> ExistUser(string name)
        {
            return await _dbUserService.GetUserOrNullAsync(name) != null;
        }

        public async Task<string> GetUserId(string name)
        {
            var user = await _dbUserService.GetUserOrNullAsync(name);
            return user?.UserId ?? "";
        }

        public TimeSpan GetUptime(string Id)
        {
            TimeSpan returnTime = TimeSpan.FromMilliseconds(0);
            try
            {
                var a = _twitchAPI.Helix.Streams.GetStreamsAsync(userIds: new List<string>() { Id }).Result;

                if (a.Streams.Length > 0)
                {
                    returnTime = DateTime.Now.ToUniversalTime() - a.Streams[0].StartedAt;
                }
            }
            catch (Exception)
            {

            }
            return returnTime;
        }
    }
}
