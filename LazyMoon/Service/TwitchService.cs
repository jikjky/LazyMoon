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

        private string GetAccessTokken(string ClientId, string ClientSecret)
        {
            String callUrl = "https://id.twitch.tv/oauth2/token";

            String postData = $"client_id={ClientId}&client_secret={ClientSecret}&grant_type=client_credentials";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(callUrl);
            // 인코딩 UTF-8
            byte[] sendData = UTF8Encoding.UTF8.GetBytes(postData);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = sendData.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(sendData, 0, sendData.Length);
            requestStream.Close();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
            string readData = streamReader.ReadToEnd();
            var parameter = readData.Split(',');
            var value = parameter[0].Split(':');
            var returnValue = value[1].Replace("\"", "");
            streamReader.Close();
            httpWebResponse.Close();
            return returnValue;
        }

        public TwitchService(TwitchAPI twitchAPI, DBUserService dbUserService, IConfiguration configuration)
        {
            _dbUserService = dbUserService;
            _twitchAPI = twitchAPI;
            _configuration = configuration;

            string oAuth = _configuration.GetValue<string>("TwitchOAuth:OAuth");
            string clientId = _configuration.GetValue<string>("TwitchOAuth:ClientId");
            string clientSecret = _configuration.GetValue<string>("TwitchOAuth:ClientSecret");
            _twitchAPI.Settings.ClientId = clientId;
            _twitchAPI.Settings.AccessToken = GetAccessTokken(clientId, clientSecret);
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
