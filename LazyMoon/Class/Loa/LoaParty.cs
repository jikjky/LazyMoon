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
    public enum EPartyCombinations
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

        public EPartyCombinations GetPlayerPartyCombinations()
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
            return StrongCharacter == CharacterClass.Dealer ? EPartyCombinations.DDDD + offset : EPartyCombinations.SDDD + offset;
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

        public Player Add(EPartyCombinations partyCombinations)
        {
            switch (partyCombinations)
            {
                case EPartyCombinations.DDDD:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Dealer]
                    });
                case EPartyCombinations.DDDS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Supporter]
                    });
                case EPartyCombinations.DDSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Supporter, CharacterClass.Supporter]
                    });
                case EPartyCombinations.DSSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = [CharacterClass.Supporter, CharacterClass.Supporter, CharacterClass.Supporter]
                    });
                case EPartyCombinations.SDDD:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Dealer]
                    });
                case EPartyCombinations.SDDS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Supporter]
                    });
                case EPartyCombinations.SDSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = [CharacterClass.Dealer, CharacterClass.Supporter, CharacterClass.Supporter]
                    });
                case EPartyCombinations.SSSS:
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

        public string PartyCombinationsToString(EPartyCombinations partyCombinations)
        {
            switch (partyCombinations)
            {
                case EPartyCombinations.DDDD:
                    return "ㄷㄷㄷㄷ";
                case EPartyCombinations.DDDS:
                    return "ㄷㄷㄷㅍ";
                case EPartyCombinations.DDSS:
                    return "ㄷㄷㅍㅍ";
                case EPartyCombinations.DSSS:
                    return "ㄷㅍㅍㅍ";
                case EPartyCombinations.SDDD:
                    return "ㅍㄷㄷㄷ";
                case EPartyCombinations.SDDS:
                    return "ㅍㄷㄷㅍ";
                case EPartyCombinations.SDSS:
                    return "ㅍㄷㅍㅍ";
                case EPartyCombinations.SSSS:
                    return "ㅍㅍㅍㅍ";
                default:
                    break;
            }
            return string.Empty;
        }

        public bool OKPartyCombinations(EPartyCombinations partyCombinations)
        {
            var needSupoter = MaxSupoter - CurrentSupoter;
            var needCombinations = MaxCombinations - CurrnetCombinations;
            int supoter;
            if (IsRaid8)
            {
                switch (partyCombinations)
                {
                    case EPartyCombinations.DDDD:
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

                    case EPartyCombinations.DDDS:
                        supoter = 1;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case EPartyCombinations.DDSS:
                        supoter = 2;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case EPartyCombinations.DSSS:
                        supoter = 3;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case EPartyCombinations.SDDD:
                        return false;

                    case EPartyCombinations.SDDS:
                        return false;

                    case EPartyCombinations.SDSS:
                        return false;

                    case EPartyCombinations.SSSS:
                        return false;
                }
            }
            //16
            else
            {
                switch (partyCombinations)
                {
                    case EPartyCombinations.DDDD:
                        supoter = 0;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needCombinations != 0;

                    case EPartyCombinations.DDDS:
                        supoter = 1;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case EPartyCombinations.DDSS:
                        supoter = 2;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case EPartyCombinations.DSSS:
                        supoter = 3;
                        if (needCombinations - 4 == 0)
                        {
                            return needSupoter == supoter;
                        }
                        return needSupoter >= supoter;

                    case EPartyCombinations.SDDD:
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

                    case EPartyCombinations.SDDS:
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

                    case EPartyCombinations.SDSS:
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

                    case EPartyCombinations.SSSS:
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

                int firstCount = 0;
                int SecondCount = 0;

                int count = 0;
                while (true)
                {

                    var stroingDealers = Players.Where(x => x.StrongCharacter == CharacterClass.Dealer && !x.OrderMessages.Contains("(ㄷ)"));
                    if (stroingDealers.Any())
                    {
                        Player stroingDealer;
                        if (rd.Next(0, 2) == 0)
                        {
                            stroingDealer = stroingDealers.First();
                        }
                        else
                        {
                            stroingDealer = stroingDealers.Last();
                        }

                            int index = DepartureOrder * 2;
                            index += rd.Next(2);
                            if (index == DepartureOrder * 2)
                            {
                                if (string.IsNullOrEmpty(stroingDealer.OrderMessages[index]))
                                {
                                    stroingDealer.OrderMessages[index] = "(ㄷ)";
                                    firstCount++;
                                }
                                else if (string.IsNullOrEmpty(stroingDealer.OrderMessages[index + 1]))
                                {
                                    stroingDealer.OrderMessages[index + 1] = "(ㄷ)";
                                    SecondCount++;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(stroingDealer.OrderMessages[index]))
                                {
                                    stroingDealer.OrderMessages[index] = "(ㄷ)";
                                    SecondCount++;
                                }
                                else if (string.IsNullOrEmpty(stroingDealer.OrderMessages[index]))
                                {
                                    stroingDealer.OrderMessages[index - 1] = "(ㄷ)";
                                    firstCount++;
                                }
                            }
                    }
                    if (firstCount == 2 && SecondCount == 2)
                    {
                        break;
                    }
                    if (count++ > 1000 || firstCount + SecondCount > 4)
                    {
                        break;
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
