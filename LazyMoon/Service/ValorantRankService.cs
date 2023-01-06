using LazyMoon.Class;
using System.Collections.Generic;
using System.IO;
using System;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using System.Threading.Tasks;

namespace LazyMoon.Service
{
    public class ValorantRankService
    {
        public Action<int, string, string> OnChangeRank;

        public int currentRank;

        public int currentScore;

        public class ValorantRating
        {
            public string MarkName;
            public string MarkImage;
        }

        public int ranking;

        public List<ValorantRating> valorantRatings = new List<ValorantRating>()
        {
            new ValorantRating() { MarkName = "Iron1", MarkImage = "ValorantRanks\\Iron1.png" },
            new ValorantRating() { MarkName = "Iron2", MarkImage = "ValorantRanks\\Iron2.png" },
            new ValorantRating() { MarkName = "Iron3", MarkImage = "ValorantRanks\\Iron3.png" },
            new ValorantRating() { MarkName = "Bronze1", MarkImage = "ValorantRanks\\Bronze1.png" },
            new ValorantRating() { MarkName = "Bronze2", MarkImage = "ValorantRanks\\Bronze2.png" },
            new ValorantRating() { MarkName = "Bronze3", MarkImage = "ValorantRanks\\Bronze3.png" },
            new ValorantRating() { MarkName = "Silver1", MarkImage = "ValorantRanks\\Silver1.png" },
            new ValorantRating() { MarkName = "Silver2", MarkImage = "ValorantRanks\\Silver2.png" },
            new ValorantRating() { MarkName = "Silver3", MarkImage = "ValorantRanks\\Silver3.png" },
            new ValorantRating() { MarkName = "Gold1", MarkImage = "ValorantRanks\\Gold1.png" },
            new ValorantRating() { MarkName = "Gold2", MarkImage = "ValorantRanks\\Gold2.png" },
            new ValorantRating() { MarkName = "Gold3", MarkImage = "ValorantRanks\\Gold3.png" },
            new ValorantRating() { MarkName = "Platinum1", MarkImage = "ValorantRanks\\Platinum1.png" },
            new ValorantRating() { MarkName = "Platinum2", MarkImage = "ValorantRanks\\Platinum2.png" },
            new ValorantRating() { MarkName = "Platinum3", MarkImage = "ValorantRanks\\Platinum3.png" },
            new ValorantRating() { MarkName = "Diamond1", MarkImage = "ValorantRanks\\Diamond1.png" },
            new ValorantRating() { MarkName = "Diamond2", MarkImage = "ValorantRanks\\Diamond2.png" },
            new ValorantRating() { MarkName = "Diamond3", MarkImage = "ValorantRanks\\Diamond3.png" },
            new ValorantRating() { MarkName = "Immortal", MarkImage = "ValorantRanks\\Immortal.png" },
            new ValorantRating() { MarkName = "Radiant", MarkImage = "ValorantRanks\\Radiant.png" }
        };


        public string chanel = "";

        private readonly TwitchBotService twitchBotService;
        private readonly DBValorantRankService dbValorantRankService;


        public ValorantRankService(TwitchBotService _twitchBotService, DBValorantRankService dbValorantRankService)
        {
            this.twitchBotService = _twitchBotService;
            this.dbValorantRankService = dbValorantRankService;
        }

        private async Task LoadData(string chanel)
        {
            this.chanel = chanel;
            var rank = await dbValorantRankService.GetValorantRankOrNullAsync(chanel);
            if (rank != null)
            {
                currentRank = rank.currentRank;
                currentScore = rank.currentScore;
            }
        }

        public async Task<bool> SetBot(string chanel)
        {
            twitchBotService.OnMessageReceived += Client_OnMessageReceived;
            twitchBotService.OnMessageReceived += OnMessageReceived;

            var result = twitchBotService.SetBot(chanel,TwitchBotService.EBotUseService.ValorantRank);
            await LoadData(chanel);
            return result;
        }

        public void ChangeRank(int score, string rank)
        {
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
                        currentScore = 0;

                }
                else if (temp >= 100)
                {
                    if (currentRank < 18)
                    {
                        currentRank++;
                        currentScore = 10;
                    }
                    else
                        currentScore = temp;
                }
                else
                    currentScore = temp;
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
            dbValorantRankService.SetValorantRankOrNullAsync(chanel, currentRank, currentScore);
            GetRank();
        }

        public void GetRank()
        {
            OnChangeRank?.Invoke(currentScore, valorantRatings[currentRank].MarkName, valorantRatings[currentRank].MarkImage);
        }

        private void SendMessage(string message)
        {
            twitchBotService.SendMessage(chanel, message);
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Channel != chanel)
                return;
            if (e.ChatMessage.Username == chanel)
            {
                if (e.ChatMessage.Message[0] == '!')
                {
                    var splitText = e.ChatMessage.Message.Split(' ');

                    int score;
                    if (splitText[0].ToLower() == "!add")
                    {
                        if (splitText.Length > 1)
                        {
                            if (int.TryParse(splitText[1], out score))
                            {
                                this.ChangeRank(score, "");
                            }
                        }
                    }
                    else if (splitText[0].ToLower() == "!sub")
                    {
                        if (splitText.Length > 1)
                        {
                            if (int.TryParse(splitText[1], out score))
                            {
                                this.ChangeRank(score * -1, "");
                            }
                        }
                    }
                    else if (splitText[0].ToLower() == "!setrank")
                    {
                        if (splitText.Length > 1)
                        {
                            this.ChangeRank(0, splitText[1]);
                        }
                    }
                }
            }
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Channel != chanel)
                return;
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
    }
}
