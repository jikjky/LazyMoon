using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace LazyMoon.Class
{
    public delegate void ChangeRankEvent(int score, string name, string image);
    public class ValorantRank : IDisposable
    {
        Log4NetManager Log = Log4NetManager.GetInstance();
        Global global = Global.GetInstance();

        public class TwitchOAuth
        {
            public string OAuth;
            public string ClientId;
            public string AccessToken;

            public TwitchOAuth(FileInfo OAuthFile)
            {
                if (OAuthFile.Exists == false)
                {
                    return;
                }
                var text = OAuthFile.OpenText();
                OAuth = text.ReadLine();
                ClientId = text.ReadLine();
                AccessToken = text.ReadLine();
                text.Close();
                return;
            }
        }

        public event ChangeRankEvent OnChangeRank;

        public int currentRank;

        public int currentScore;

        public class ValorantRating
        {
            public string MarkName;
            public string MarkImage;
        }

        public int ranking;

        public List<ValorantRating> valorantRatings = new List<ValorantRating>();

        TwitchClient client;

        TwitchOAuth twitchOauth;

        public string chanel = "";

        private string mLastMessage = "";

        public ValorantRank()
        {
            Log.ValorantRankLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Create ValorantRankRoute Instance");

            valorantRatings.Add(new ValorantRating() { MarkName = "Iron1", MarkImage = "ValorantRanks\\Iron1.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Iron2", MarkImage = "ValorantRanks\\Iron2.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Iron3", MarkImage = "ValorantRanks\\Iron3.png" });

            valorantRatings.Add(new ValorantRating() { MarkName = "Bronze1", MarkImage = "ValorantRanks\\Bronze1.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Bronze2", MarkImage = "ValorantRanks\\Bronze2.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Bronze3", MarkImage = "ValorantRanks\\Bronze3.png" });

            valorantRatings.Add(new ValorantRating() { MarkName = "Silver1", MarkImage = "ValorantRanks\\Silver1.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Silver2", MarkImage = "ValorantRanks\\Silver2.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Silver3", MarkImage = "ValorantRanks\\Silver3.png" });

            valorantRatings.Add(new ValorantRating() { MarkName = "Gold1", MarkImage = "ValorantRanks\\Gold1.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Gold2", MarkImage = "ValorantRanks\\Gold2.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Gold3", MarkImage = "ValorantRanks\\Gold3.png" });

            valorantRatings.Add(new ValorantRating() { MarkName = "Platinum1", MarkImage = "ValorantRanks\\Platinum1.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Platinum2", MarkImage = "ValorantRanks\\Platinum2.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Platinum3", MarkImage = "ValorantRanks\\Platinum3.png" });

            valorantRatings.Add(new ValorantRating() { MarkName = "Diamond1", MarkImage = "ValorantRanks\\Diamond1.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Diamond2", MarkImage = "ValorantRanks\\Diamond2.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Diamond3", MarkImage = "ValorantRanks\\Diamond3.png" });

            valorantRatings.Add(new ValorantRating() { MarkName = "Immortal", MarkImage = "ValorantRanks\\Immortal.png" });
            valorantRatings.Add(new ValorantRating() { MarkName = "Radiant", MarkImage = "ValorantRanks\\Radiant.png" });
        }

        public void LoadData(string chanel)
        {
            this.chanel = chanel;
            if (global.ValorantRankDictionary.ContainsKey(chanel) == true)
            {
                currentRank = global.ValorantRankDictionary[chanel].currentRank;
                currentScore = global.ValorantRankDictionary[chanel].currentScore;
            }
        }

        public bool SetBot(string chanel)
        {
            Log.ValorantRankLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Set Instance Chanel : " + chanel);

            this.chanel = chanel;

            twitchOauth = new TwitchOAuth(new FileInfo("wwwroot//twitch.oauth"));
            try
            {
                Log.ValorantRankLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Set Client Option");
                ConnectionCredentials credentials = new ConnectionCredentials(chanel, twitchOauth.OAuth);
                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                };
                WebSocketClient customClient = new WebSocketClient(clientOptions);
                Log.ValorantRankLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Create Client");
                client = new TwitchClient(customClient);
                client.Initialize(credentials, this.chanel);

                client.OnMessageReceived += Client_OnMessageReceived;

                Log.ValorantRankLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Connect Client");
                client.Connect();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ChangeRank(int score, string rank)
        {
            Log.ValorantRankLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Chanel : " + chanel + " Change Rank Score : " + score.ToString() + " Rank : " + rank);

            if (score != 0)
            {
                int temp = currentScore + score;

                if (temp < 0)
                {
                    if (currentRank != 0)
                    {
                        currentRank--;
                        currentScore = 90;
                    }
                    else
                    {
                        currentScore = 0;
                    }

                }
                else if (temp >= 100)
                {
                    if (currentRank < 18)
                    {
                        currentRank++;
                        currentScore = 10;
                    }
                    else
                    {
                        currentScore = temp;
                    }
                }
                else
                {
                    currentScore = temp;
                }
            }
            if (rank != "")
            {
                int i = 0;
                foreach (var name in valorantRatings)
                {
                    if (name.MarkName.ToLower() == rank.ToLower())
                    {
                        currentRank = i;
                        break;
                    }
                    i++;
                }
            }
            global.SetRank(chanel, currentRank);
            global.SetScore(chanel, currentScore);
            global.SaveRankInfo();
            GetRank();
        }

        public void GetRank()
        {
            Log.ValorantRankLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Chanel : " + chanel + " GetRank Name : " + valorantRatings[currentRank].MarkName);
            OnChangeRank?.Invoke(currentScore, valorantRatings[currentRank].MarkName, valorantRatings[currentRank].MarkImage);
        }

        private void SendMessage(string message)
        {
            try
            {
                if (mLastMessage != message)
                {
                    client.SendMessage(chanel, message);
                }
                else
                {
                    message = message + "ㅤ";
                    client.SendMessage(chanel, message);
                }
            }
            catch (Exception e)
            {
                Log.ValorantRankLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "SendMessage Chanel Error : " + chanel + " Message : " + message + " Exception : " + e.Message);
                client.JoinChannel(chanel);
                client.SendMessage(chanel, message);
            }

            Log.ValorantRankLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SendMessage Chanel : " + chanel + " Message : " + message);
            mLastMessage = message;
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string speechText = e.ChatMessage.Message;
            Console.WriteLine(e.ChatMessage.Username);
            if (e.ChatMessage.Username == chanel)
            {
                if (e.ChatMessage.Message[0] == '!')
                {
                    var splitText = e.ChatMessage.Message.Split(' ');
                    bool isError = false;

                    int score;
                    if (splitText[0].ToLower() == "!add")
                    {
                        if (splitText.Length > 1)
                        {
                            if (int.TryParse(splitText[1], out score))
                            {
                                this.ChangeRank(score, "");
                            }
                            else
                                isError = true;
                        }
                        else
                            isError = true;
                        if (isError)
                            SendMessage($"!add [0~2147483647]");
                    }
                    else if (splitText[0].ToLower() == "!sub")
                    {
                        if (splitText.Length > 1)
                        {
                            if (int.TryParse(splitText[1], out score))
                            {
                                this.ChangeRank(score * -1, "");
                            }
                            else
                                isError = true;
                        }
                        else
                            isError = true;
                        if (isError)
                            SendMessage($"!sub [0~2147483647]");
                    }
                    else if (splitText[0].ToLower() == "!setrank")
                    {
                        if (splitText.Length > 1)
                        {
                            this.ChangeRank(0, splitText[1]);
                        }
                        else
                            isError = true;
                        if (isError)
                            SendMessage($"!setrank [rank name]");
                    }
                    else if (splitText[0].ToLower() == "!help")
                    {
                        SendMessage($"add, sub, setrank");
                    }
                }

            }
        }
        public void Dispose()
        {
            if (client != null)
            {
                if (client.IsConnected == true)
                {
                    client.Disconnect();
                }
            }
        }
    }
}
