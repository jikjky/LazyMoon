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
        public List<string> OrderMessages { get; set; } = new List<string>();
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
        public List<CharacterClass> WeekCharacter { get; set; } = new List<CharacterClass>();

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
            if (WeekCharacter.Where(x => x == CharacterClass.Dealer).Count() == 0)
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

        public bool IsRaid8 { get; set; } = false;

        public bool StrongSupoter { get; set; } = false;
        public List<Player> Players { get; set; } = new List<Player>();

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
                        WeekCharacter = new List<CharacterClass>() { CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Dealer }
                    });
                case PartyCombinations.DDDS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = new List<CharacterClass>() { CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Supporter }
                    });
                case PartyCombinations.DDSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = new List<CharacterClass>() { CharacterClass.Dealer, CharacterClass.Supporter, CharacterClass.Supporter }
                    });
                case PartyCombinations.DSSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Dealer,
                        WeekCharacter = new List<CharacterClass>() { CharacterClass.Supporter, CharacterClass.Supporter, CharacterClass.Supporter }
                    });
                case PartyCombinations.SDDD:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = new List<CharacterClass>() { CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Dealer }
                    });
                case PartyCombinations.SDDS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = new List<CharacterClass>() { CharacterClass.Dealer, CharacterClass.Dealer, CharacterClass.Supporter }
                    });
                case PartyCombinations.SDSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = new List<CharacterClass>() { CharacterClass.Dealer, CharacterClass.Supporter, CharacterClass.Supporter }
                    });
                case PartyCombinations.SSSS:
                    return Add(new Player()
                    {
                        StrongCharacter = CharacterClass.Supporter,
                        WeekCharacter = new List<CharacterClass>() { CharacterClass.Supporter, CharacterClass.Supporter, CharacterClass.Supporter }
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
            int supoter = 0;
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

        public int DepartureOrder { get; set; } = 0;

        public string Make()
        {
            if (CurrnetCombinations == MaxCombinations)
            {
                foreach (var item in Players)
                {
                    item.OrderMessages = new List<string>() { "", "", "", "" };
                    item.UsedSupoterCount = 0;
                }

                if (IsRaid8)
                {
                    var weekSupoterFindAll = Players.FindAll(x => x.SupoterCount != 0 && (x.SupoterCount > x.UsedSupoterCount));
                    var weekSupoter = weekSupoterFindAll[weekSupoterFindAll.Count - 1];
                    int tempDepartureOrder = DepartureOrder * 2;
                    weekSupoter.OrderMessages[tempDepartureOrder] = "ㅍ";
                    weekSupoter.UsedSupoterCount++;
                    int strongDealerCounter = 0;
                    if (tempDepartureOrder == 0)
                    {
                        int tempCount = 0;
                        while (true)
                        {
                            if (weekSupoter.SupoterCount > weekSupoter.UsedSupoterCount + (tempCount++))
                            {
                                continue;
                            }
                            weekSupoter.OrderMessages[tempDepartureOrder + 1] = "(ㄷ)";
                            break;

                        }
                    }
                    else
                    {
                        weekSupoter.OrderMessages[tempDepartureOrder + 1] = "(ㄷ)";
                    }
                    foreach (var item in Players)
                    {
                        if (!item.Equals(weekSupoter))
                        {
                            if (strongDealerCounter == 2)
                            {
                                item.OrderMessages[tempDepartureOrder + 1] = "(ㄷ)";
                            }
                            else
                            {
                                item.OrderMessages[tempDepartureOrder] = "(ㄷ)";
                            }
                            strongDealerCounter++;
                        }
                    }
                    for (var i = 0; i < 3; i++)
                    {
                        int index = i;
                        if (tempDepartureOrder <= i)
                        {
                            index++;
                        }
                        weekSupoterFindAll = Players.FindAll(x => x.SupoterCount != 0 && (x.SupoterCount > x.UsedSupoterCount));
                        if (tempDepartureOrder == 0)
                        {
                            weekSupoter = weekSupoterFindAll[0];
                        }
                        else
                        {
                            weekSupoter = weekSupoterFindAll[weekSupoterFindAll.Count - 1];
                        }
                        if (weekSupoter.OrderMessages[index] == "(ㄷ)")
                        {
                            if (tempDepartureOrder == 0)
                            {
                                Players[tempDepartureOrder].OrderMessages[index] = "(ㄷ)";
                                Players[tempDepartureOrder].OrderMessages[index - 1] = "";
                                weekSupoter.OrderMessages[index - 1] = "(ㄷ)";
                            }
                            else
                            {
                                Players[tempDepartureOrder - 1].OrderMessages[index] = "(ㄷ)";
                                Players[tempDepartureOrder - 1].OrderMessages[index - 1] = "";
                                weekSupoter.OrderMessages[index - 1] = "(ㄷ)";
                            }
                        }
                        weekSupoter.OrderMessages[index] = "ㅍ";
                        weekSupoter.UsedSupoterCount++;
                        foreach (var item in Players)
                        {
                            if (!item.Equals(weekSupoter))
                            {
                                if (string.IsNullOrEmpty(item.OrderMessages[index]))
                                {
                                    item.OrderMessages[index] = "ㄷ";
                                }
                            }
                        }
                    }
                    //문자 만듬
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
                else
                {
                    var stroingSupoterFindAll = Players.FindAll(x => x.StrongCharacter == CharacterClass.Supporter);
                    if (stroingSupoterFindAll.Count() == 1)
                    {
                        var stroingSupoter = stroingSupoterFindAll[0];
                        stroingSupoter.OrderMessages[DepartureOrder] = "(ㅍ)";
                        stroingSupoter.UsedSupoterCount++;
                        foreach (var item in Players)
                        {
                            if (!item.Equals(stroingSupoter))
                            {
                                item.OrderMessages[DepartureOrder] = "(ㄷ)";
                            }
                        }
                    }
                    else
                    {
                        var weekSupoterFindAll = Players.FindAll(x => x.SupoterCount != 0 && (x.SupoterCount > x.UsedSupoterCount));
                        var weekSupoter = weekSupoterFindAll[weekSupoterFindAll.Count - 1];
                        weekSupoter.OrderMessages[DepartureOrder] = "ㅍ";
                        weekSupoter.UsedSupoterCount++;
                        if (DepartureOrder == 0)
                        {
                            int tempCount = 0;
                            while (true)
                            {
                                if (weekSupoter.SupoterCount > weekSupoter.UsedSupoterCount + (tempCount++))
                                {
                                    continue;
                                }
                                weekSupoter.OrderMessages[DepartureOrder + tempCount] = "(ㄷ)";
                                break;

                            }
                        }
                        else
                        {
                            weekSupoter.OrderMessages[DepartureOrder - 1] = "(ㄷ)";
                        }
                        foreach (var item in Players)
                        {
                            if (!item.Equals(weekSupoter))
                            {
                                item.OrderMessages[DepartureOrder] = "(ㄷ)";
                            }
                        }
                    }
                    for (var i = 0; i < 3; i++)
                    {
                        int index = i;
                        if (DepartureOrder <= i)
                        {
                            index++;
                        }
                        var weekSupoterFindAll = Players.FindAll(x => x.SupoterCount != 0 && (x.SupoterCount > x.UsedSupoterCount));
                        var weekSupoter = weekSupoterFindAll[0];
                        weekSupoter.OrderMessages[index] = "ㅍ";
                        weekSupoter.UsedSupoterCount++;
                        foreach (var item in Players)
                        {
                            if (!item.Equals(weekSupoter))
                            {
                                if (string.IsNullOrEmpty(item.OrderMessages[index]))
                                {
                                    item.OrderMessages[index] = "ㄷ";
                                }
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
                            result += message;
                            orderIndex++;
                        }
                        playerIndex++;
                    }
                    return result;
                }
            }
            return string.Empty;
        }
    }
}
