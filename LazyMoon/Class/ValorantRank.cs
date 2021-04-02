using System;
using System.Collections.Generic;
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
    public delegate void ChangeRankEvent(int score, string rank, string path);

    public class ValorantRank
    {
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

        public ValorantRank()
        {
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
            GetRank();
        }

        public void GetRank()
        {
            OnChangeRank?.Invoke(currentScore, valorantRatings[currentRank].MarkName, valorantRatings[currentRank].MarkImage);
        }
    }
}
