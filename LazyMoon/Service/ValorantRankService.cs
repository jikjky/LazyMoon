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
using System.Net.Http;
using System.Linq;
using System.Diagnostics;

namespace LazyMoon.Service
{
    public class ValorantRankService
    {
        public Action<int, string, string> OnChangeRank;

        public string NickName = "";

        public string Tag = "";

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
            new ValorantRating() { MarkName = "Ascendant1", MarkImage = "ValorantRanks\\Ascendant1.png" },
            new ValorantRating() { MarkName = "Ascendant2", MarkImage = "ValorantRanks\\Ascendant2.png" },
            new ValorantRating() { MarkName = "Ascendant3", MarkImage = "ValorantRanks\\Ascendant3.png" },
            new ValorantRating() { MarkName = "Immortal", MarkImage = "ValorantRanks\\Immortal.png" },
            new ValorantRating() { MarkName = "Radiant", MarkImage = "ValorantRanks\\Radiant.png" }
        };


        public string chanel = "";

        private readonly TwitchBotService twitchBotService;
        private readonly DBValorantRankService dbValorantRankService;
        private readonly IHttpClientFactory clientFactory;



        public ValorantRankService(TwitchBotService _twitchBotService, DBValorantRankService dbValorantRankService, IHttpClientFactory _clientFactory)
        {
            this.twitchBotService = _twitchBotService;
            this.dbValorantRankService = dbValorantRankService;
            this.clientFactory = _clientFactory;
        }

        private async Task LoadData(string chanel)
        {
            this.chanel = chanel;
            var rank = await dbValorantRankService.GetValorantRankOrNullAsync(chanel);
            if (rank != null)
            {
                NickName = rank.NickName;
                Tag = rank.Tag;
                await GetRank();
            }
        }

        public async Task<bool> SetBot(string chanel)
        {
            var result = twitchBotService.SetBot(chanel, TwitchBotService.EBotUseService.ValorantRank);
            await LoadData(chanel);
            return result;
        }

        public async Task ChangeRank(string nickName, string tag)
        {
            await dbValorantRankService.SetValorantRankOrNullAsync(chanel, nickName, tag);
            await GetRank();
        }

        public async Task GetRank()
        {
            try
            {
                if (!string.IsNullOrEmpty(NickName) && !string.IsNullOrEmpty(Tag))
                {
                    var httpClient = clientFactory.CreateClient();
                    var text = await httpClient.GetStringAsync($"https://api.yash1441.repl.co/valorant/kr/{NickName}/{Tag}?onlyRank=true");
                    if (text == "null - null RR" || text == "Error 404")
                        return;
                    text = text.Replace("RR", "");
                    text = text.Replace(" ", "");
                    var splitList = text.Split("-");
                    var currentRank = valorantRatings.IndexOf(valorantRatings.Single(x => x.MarkName == splitList[0]));
                    var currentScore = Convert.ToInt32(splitList[1]);

                    OnChangeRank?.Invoke(currentScore, valorantRatings[currentRank].MarkName, valorantRatings[currentRank].MarkImage);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
