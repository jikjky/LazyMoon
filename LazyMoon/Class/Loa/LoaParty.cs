using Microsoft.IdentityModel.Tokens;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyMoon.Class.Loa
{
    public enum CharacterClass
    {
        Dealer,
        Supporter
    }

    public enum PartyCombinations
    {
        DDDD = 0,
        DDDS = 1,
        DDSS = 2,
        DSSS = 3,
        SDDD = 10,
        SDDS = 11,
        SDSS = 12,
        SSSS = 13,
    }

    public class Player
    {
        public List<string> OrderMessages { get; set; }
        public int UsedSupoterCount { get; set; } = 0;
        public int SupoterCount
        {
            get
            {
                int result = 0;
                if (StrongCharacter == CharacterClass.Supporter)
                {
                    result++;
                }

                foreach (CharacterClass characterClass in WeekCharacter)
                {
                    if (characterClass == CharacterClass.Supporter)
                    {
                        result++;
                    }
                }


                return result;
            }
        }
        public CharacterClass StrongCharacter { get; set; }
        public List<CharacterClass> WeekCharacter { get; set; } = [];

        public PartyCombinations GetPlayerPartyCombinations()
        {
            int offset = 0;
            if (WeekCharacter.Where(x => x == CharacterClass.Dealer).Count() == 3)
            {
                offset = 0;
            }
            if (WeekCharacter.Where(x => x == CharacterClass.Dealer).Count() == 2)
            {
                offset = 1;
            }
            if (WeekCharacter.Where(x => x == CharacterClass.Dealer).Count() == 1)
            {
                offset = 2;
            }
            if (!WeekCharacter.Where(x => x == CharacterClass.Dealer).Any())
            {
                offset = 3;
            }
            return StrongCharacter == CharacterClass.Dealer ? PartyCombinations.DDDD + offset : PartyCombinations.SDDD + offset;
        }
    }
    public class LoaParty
    {
        public int CurrentSupoter { get; set; } = 0;
        public int MaxSupoter { get; set; } = 4;
        public int CurrnetCombinations { get; set; } = 0;
        public int MaxCombinations { get; set; } = 16;
        public int DepartureOrder { get; set; } = 0;
        public bool IsRaid8 { get; set; } = false;

        public bool StrongSupoter { get; set; } = false;
        public List<Player> Players { get; set; } = [];

        public Player Add(Player player)
        {
            if (player.StrongCharacter == CharacterClass.Supporter)
            {
                StrongSupoter = true;
            }
            CurrentSupoter += player.SupoterCount;
            CurrnetCombinations += 4;
            Players.Add(player);
            return player;
        }

        public Player Add(PartyCombinations partyCombinations)
        {
            switch (partyCombinations)
            {
                case PartyCombinations.DDDD:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Dealer]
                    });
                case PartyCombinations.DDDS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Supporter]
                    });
                case PartyCombinations.DDSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Supporter, CharacterClass.Supporter]
                    });
                case PartyCombinations.DSSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = [CharacterClass.Supporter, CharacterClass.Supporter, CharacterClass.Supporter]
                    });
                case PartyCombinations.SDDD:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Dealer]
                    });
                case PartyCombinations.SDDS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Supporter]
                    });
                case PartyCombinations.SDSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Supporter, CharacterClass.Supporter]
                    });
                case PartyCombinations.SSSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = [CharacterClass.Supporter, CharacterClass.Supporter, CharacterClass.Supporter]
                    });
                default:
                    break;
            }
            return new Player();
        }

        public void Remove(Player player)
        {
            if (player.StrongCharacter == CharacterClass.Supporter)
            {
                StrongSupoter = false;
            }
            CurrentSupoter -= player.SupoterCount;
            CurrnetCombinations -= 4;
            Players.Remove(player);
        }

        public string PartyCombinationsToString(Player player)
        {
            return PartyCombinationsToString(player.GetPlayerPartyCombinations());
        }

        public string PartyCombinationsToString(PartyCombinations partyCombinations)
        {
            switch (partyCombinations)
            {
                case PartyCombinations.DDDD:
                    return "ㄷㄷㄷㄷ";
                case PartyCombinations.DDDS:
                    return "ㄷㄷㄷㅍ";
                case PartyCombinations.DDSS:
                    return "ㄷㄷㅍㅍ";
                case PartyCombinations.DSSS:
                    return "ㄷㅍㅍㅍ";
                case PartyCombinations.SDDD:
                    return "ㅍㄷㄷㄷ";
                case PartyCombinations.SDDS:
                    return "ㅍㄷㄷㅍ";
                case PartyCombinations.SDSS:
                    return "ㅍㄷㅍㅍ";
                case PartyCombinations.SSSS:
                    return "ㅍㅍㅍㅍ";
                default:
                    break;
            }
            return string.Empty;
        }

        public bool OKPartyCombinations(PartyCombinations partyCombinations)
        {
            var needSupoter = MaxSupoter - CurrentSupoter;
            var needCombinations = MaxCombinations - CurrnetCombinations;
            int supoter;
            if (IsRaid8)
            {
                switch (partyCombinations)
                {
                    case PartyCombinations.DDDD:
                        supoter = 0;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        var a = needCombinations / 4;
                        if (a == 2 && needSupoter == 4)
                        {
                            return false;
                        }
                        return needCombinations != 0;

                    case PartyCombinations.DDDS:
                        supoter = 1;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case PartyCombinations.DDSS:
                        supoter = 2;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case PartyCombinations.DSSS:
                        supoter = 3;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case PartyCombinations.SDDD:
                        return false;

                    case PartyCombinations.SDDS:
                        return false;

                    case PartyCombinations.SDSS:
                        return false;

                    case PartyCombinations.SSSS:
                        return false;
                }
            }
            //16
            else
            {
                switch (partyCombinations)
                {
                    case PartyCombinations.DDDD:
                        supoter = 0;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needCombinations != 0;

                    case PartyCombinations.DDDS:
                        supoter = 1;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case PartyCombinations.DDSS:
                        supoter = 2;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case PartyCombinations.DSSS:
                        supoter = 3;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case PartyCombinations.SDDD:
                        if (StrongSupoter == true)
                        {
                            return false;
                        }
                        supoter = 1;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case PartyCombinations.SDDS:
                        if (StrongSupoter == true)
                        {
                            return false;
                        }
                        supoter = 2;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case PartyCombinations.SDSS:
                        if (StrongSupoter == true)
                        {
                            return false;
                        }
                        supoter = 3;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case PartyCombinations.SSSS:
                        if (StrongSupoter == true)
                        {
                            return false;
                        }
                        supoter = 4;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;
                }
            }
            return false;
        }

        public string Make()
        {
            if (CurrnetCombinations == MaxCombinations)
            {
                foreach (var item in Players)
                {
                    item.OrderMessages = ["", "", "", ""];
                    item.UsedSupoterCount = 0;
                }
                return IsRaid8 ? Make8() : Make16();
            }
            return string.Empty;
        }

        private string Make8()
        {
            int supoterIndex = DepartureOrder;
            int tempIndex;
            tempIndex = DepartureOrder;
            Random rd = new();
            while (true)
            {
                
                while (true)
                {
                    var supotersPlayers = Players.Where(x => x.SupoterCount - x.UsedSupoterCount != 0);
                    if (supotersPlayers.Any())
                    {
                        Player supotersPlayer;
                        if (rd.Next(0, 2) == 0)
                        {
                            supotersPlayer = supotersPlayers.First();
                        }
                        else
                        {
                            supotersPlayer = supotersPlayers.Last();
                        }
                        supotersPlayer.OrderMessages[supoterIndex] = "ㅍ";
                        supotersPlayer.UsedSupoterCount++;
                        supoterIndex = supoterIndex < 3 ? supoterIndex + 1 : 0;
                        continue;
                    }
                    break;
                }

                var stroingDealers = Players.Where(x => x.StrongCharacter == CharacterClass.Dealer);
                int firstCount = 0;
                int SecondCount = 0;

                if (stroingDealers.Any())
                {
                    int count = 0;
                    foreach (var stroingDealer in stroingDealers)
                    {
                        int index = DepartureOrder * 2;
                        while (true)
                        {
                            if (count % 2 == 0)
                            {
                                if (string.IsNullOrEmpty(stroingDealer.OrderMessages[index]))
                                {
                                    stroingDealer.OrderMessages[index] = "(ㄷ)";
                                    firstCount++;
                                    break;
                                }
                                else
                                {
                                    stroingDealer.OrderMessages[index + 1] = "(ㄷ)";
                                    SecondCount++;
                                    break;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(stroingDealer.OrderMessages[index + 1]))
                                {
                                    stroingDealer.OrderMessages[index + 1] = "(ㄷ)";
                                    SecondCount++;
                                    break;
                                }
                                else
                                {
                                    stroingDealer.OrderMessages[index] = "(ㄷ)";
                                    firstCount++;
                                    break;
                                }
                            }
                        }
                        count++;
                    }
                }

                if (firstCount != 2 || SecondCount != 2)
                {
                    foreach (var item in Players)
                    {
                        item.OrderMessages = ["", "", "", ""];
                        item.UsedSupoterCount = 0;
                    }
                    tempIndex = tempIndex < 3 ? tempIndex + 1 : 0;
                    supoterIndex = tempIndex;
                    continue;
                }
                break;
            }

            foreach (var player in Players)
            {
                for (int i = 0; i < player.OrderMessages.Count; i++)
                {
                    if (string.IsNullOrEmpty(player.OrderMessages[i]))
                    {
                        player.OrderMessages[i] = "ㄷ";
                    }
                }
            }

            string result = string.Empty;
            int playerIndex = 1;
            foreach (var item in Players)
            {
                if (playerIndex == 1)
                {
                    result += $"{DepartureOrder + 1}-{playerIndex}";
                }
                else
                {
                    result += $"  {DepartureOrder + 1}-{playerIndex}";
                }
                int orderIndex = 1;
                foreach (var message in item.OrderMessages)
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        result += "ㄷ";
                    }
                    else
                    {
                        result += message;
                    }
                    orderIndex++;
                }
                playerIndex++;
            }
            return result;
        }

        private string Make16()
        {
            var stroingSuppoters = Players.Where(x => x.StrongCharacter == CharacterClass.Supporter);
            int supoterIndex = DepartureOrder;
            if (stroingSuppoters.Any())
            {
                var stroingSuppoter = stroingSuppoters.First();
                stroingSuppoter.OrderMessages[supoterIndex] = "(ㅍ)";
                supoterIndex = supoterIndex < 3 ? supoterIndex + 1 : 0;
                stroingSuppoter.UsedSupoterCount++;
            }
            while (true)
            {
                var supotersPlayers = Players.Where(x => x.SupoterCount - x.UsedSupoterCount != 0);
                if (supotersPlayers.Any())
                {
                    var supotersPlayer = supotersPlayers.First();
                    supotersPlayer.OrderMessages[supoterIndex] = "ㅍ";
                    supotersPlayer.UsedSupoterCount++;
                    supoterIndex = supoterIndex < 3 ? supoterIndex + 1 : 0;
                    continue;
                }
                break;
            }

            var stroingDealers = Players.Where(x => x.StrongCharacter == CharacterClass.Dealer);
            if (stroingDealers.Any())
            {
                foreach (var stroingDealer in stroingDealers)
                {
                    int index = DepartureOrder;
                    while (true)
                    {
                        if (string.IsNullOrEmpty(stroingDealer.OrderMessages[index]))
                        {
                            stroingDealer.OrderMessages[index] = "(ㄷ)";
                            break;
                        }
                        index = index < 3 ? index + 1 : 0;
                    }
                }
            }
            foreach (var player in Players)
            {
                for (int i = 0; i < player.OrderMessages.Count; i++)
                {
                    if (string.IsNullOrEmpty(player.OrderMessages[i]))
                    {
                        player.OrderMessages[i] = "ㄷ";
                    }
                }
            }

            string result = string.Empty;
            int playerIndex = 1;
            foreach (var item in Players)
            {
                if (playerIndex == 1)
                {
                    result += $"{DepartureOrder + 1}-{playerIndex}";
                }
                else
                {
                    result += $"  {DepartureOrder + 1}-{playerIndex}";
                }
                int orderIndex = 1;
                foreach (var message in item.OrderMessages)
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        result += "ㄷ";
                    }
                    else
                    {
                        result += message;
                    }
                    orderIndex++;
                }
                playerIndex++;
            }
            return result;
        }
    }
}
