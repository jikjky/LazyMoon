using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Text;
using System.Linq;
using static LazyMoon.Class.Loa.LoaBracelets;
using LazyMoon.Class.Loa;

namespace LazyMoon.Pages.LostArk
{
    public partial class BraceletsConvert
    {
        public bool IsArtifact { get; set; } = false;
        public bool CanUpgrade { get; set; } = false;

        int ConvertCount { get; set; } = 0;
        int MaxConvertCount { get; set; } = 4;
        int SpecialConvertCount { get; set; } = 0;
        int MaxSpecialConvertCount { get; set; } = 3;


        class PageEffectItem
        {
            public required EffectItem EffectItem { get; set; }
            public bool Lock { get; set; }
            public required string Text { get; set; }
            public required string Grade { get; set; }

        }

        List<string> fixedEffectList = ["2개", "1개"];

        private string FixedEffect
        {
            get => fixedEffect;
            set
            {
                fixedEffect = value;
                EffectCount(true);
            }
        }
        private string fixedEffect = "2개";

        List<string> grantEffectList = ["3개", "2개"];
        private string GrantEffect
        {
            get => grantEffect;
            set
            {
                grantEffect = value;
                EffectCount(false);
            }
        }
        private string grantEffect = "3개";

        readonly List<string> selectStringEffectList = [];
        readonly List<EffectType> selectEffectList = [];

        List<PageEffectItem> pageEffectItems = [
            new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
            new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
            new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,];

        private string FixedStringValue1 { get; set; } = string.Empty;
        private string FixedStringValue2 { get; set; } = string.Empty;

        private string BottomFixedStringValue1 { get; set; } = string.Empty;
        private string BottomFixedStringValue2 { get; set; } = string.Empty;

        private bool IsFixedStringCount2 { get; set; } = true;

        public int FixedCount { get; private set; } = 2;
        public int GrantCount { get; private set; } = 3;



        protected override void OnAfterRender(bool firstRender)
        {
            MakeStringList(false);
            base.OnAfterRender(firstRender);
        }

        public void MakeStringList(bool bArtifact)
        {
            selectEffectList.Clear();
            selectStringEffectList.Clear();
            LoaBracelets loaBracelets = new();
            foreach (var item in loaBracelets.basicEffectTypes)
            {
                selectEffectList.Add(item);
            }
            foreach (var item in loaBracelets.combatEffectTypes)
            {
                selectEffectList.Add(item);
            }
            int index = 0;
            foreach (var item in loaBracelets.specialEffectTypes)
            {
                selectEffectList.Add(item);
                if (index > 28)
                {
                    break;
                }
                index++;
            }

            foreach (var item in selectEffectList)
            {
                if (item.EffectTypeEnum == EEffectType.특수효과)
                {
                    if (bArtifact && IsUpgrading == false)
                    {
                        selectStringEffectList.Add(item.Name1);
                    }
                    else
                    {
                        selectStringEffectList.Add(item.Name2);
                    }
                }
                else
                {
                    selectStringEffectList.Add(item.EffectTypeEnum.ToString());
                }
            }
        }

        public void ItemTypeClick(bool bArtifact)
        {
            IsArtifact = bArtifact;
            if (bArtifact)
            {
                fixedEffectList = ["2개", "1개"];
                grantEffectList = ["2개", "1개"];
                FixedEffect = "2개";
                GrantEffect = "2개";
                MakeStringList(true);
                CanUpgrade = true;
            }
            else
            {
                fixedEffectList = ["2개", "1개"];
                grantEffectList = ["3개", "2개"];
                FixedEffect = "2개";
                GrantEffect = "3개";
                MakeStringList(false);
                CanUpgrade = false;
            }
        }

        public void EffectCount(bool bFixedEffect)
        {
            if (bFixedEffect)
            {
                if (FixedEffect == "2개")
                {
                    IsFixedStringCount2 = true;
                    FixedStringValue1 = string.Empty;
                    FixedStringValue2 = string.Empty;
                    FixedCount = 2;
                }
                else
                {
                    IsFixedStringCount2 = false;
                    FixedStringValue1 = string.Empty;
                    FixedStringValue2 = string.Empty;
                    FixedCount = 1;
                }
            }
            else
            {
                if (GrantEffect == "3개")
                {
                    GrantCount = 3;
                    pageEffectItems =
            [
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
            ];
                }
                if (GrantEffect == "2개")
                {
                    GrantCount = 2;
                    pageEffectItems =
            [
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
            ];
                }
                if (GrantEffect == "1개")
                {
                    GrantCount = 1;
                    pageEffectItems =
            [
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
            ];
                }
            }
        }

        public bool IsUpgrading { get; set; }

        public void Upgrade()
        {
            ConvertCount = 0;
            CanUpgrade = false;
            IsUpgrading = true;
            foreach (var item in pageEffectItems)
            {
                item.Lock = true;
            }
            pageEffectItems.Add(new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" });
            ConvertClick();
        }

        public void InitializeClick()
        {
            if (IsArtifact)
            {
                CanUpgrade = true;
                BottomFixedStringValue1 = FixedStringValue1;
                BottomFixedStringValue2 = FixedStringValue2;
            }
            IsUpgrading = false;
            MakeStringList(IsArtifact);
            if (GrantEffect == "3개")
            {
                GrantCount = 3;
                pageEffectItems =
            [
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
            ];
            }
            if (GrantEffect == "2개")
            {
                GrantCount = 2;
                pageEffectItems =
            [
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
            ];
            }
            if (GrantEffect == "1개")
            {
                GrantCount = 1;
                pageEffectItems =
            [
                new PageEffectItem() { EffectItem = new EffectItem(), Grade = "", Text = "부여효과" } ,
            ];
            }
            ConvertCount = 0;
            SpecialConvertCount = 0;
        }

        public void ConvertClick(bool bSpecialConvertCount = false)
        {
            LoaBracelets bracelets = new();

            static void InitBracelets(LoaBracelets bracelets, string text)
            {
                EBraceletsEffectType braceletsEffectEnum = EBraceletsEffectType.None;
                EEffectType effectTypeEnum = EEffectType.None;
                foreach (EEffectType item in Enum.GetValues(typeof(EEffectType)))
                {
                    if (text == item.ToString() || text == "힘")
                    {
                        if (text == "힘")
                        {
                            effectTypeEnum = EEffectType.힘;
                        }
                        else
                        {
                            effectTypeEnum = item;
                        }
                        break;
                    }
                }
                braceletsEffectEnum = effectTypeEnum switch
                {
                    EEffectType.힘 => EBraceletsEffectType.기본효과,
                    EEffectType.체력 => EBraceletsEffectType.기본효과,
                    EEffectType.치명 => EBraceletsEffectType.전투특성,
                    EEffectType.특화 => EBraceletsEffectType.전투특성,
                    EEffectType.제압 => EBraceletsEffectType.전투특성,
                    EEffectType.신속 => EBraceletsEffectType.전투특성,
                    EEffectType.인내 => EBraceletsEffectType.전투특성,
                    EEffectType.숙련 => EBraceletsEffectType.전투특성,
                    EEffectType.None => EBraceletsEffectType.None,
                    _ => EBraceletsEffectType.특수효과,
                };
                if (braceletsEffectEnum == EBraceletsEffectType.None && !string.IsNullOrEmpty(text))
                {
                    braceletsEffectEnum = EBraceletsEffectType.특수효과;
                    effectTypeEnum = EEffectType.특수효과;
                }
                bracelets.EffectItems.Add(new LoaBracelets.EffectItem() { BraceletsEffectEnum = braceletsEffectEnum, Name = text, EffectTypeEnum = effectTypeEnum, IsNull = false });
            }
            string value1 = FixedStringValue1;
            string value2 = FixedStringValue2;

            if (IsUpgrading)
            {
                if (bracelets.specialEffectTypes.Any(x => x.Name1 == FixedStringValue1))
                {
                    value1 = bracelets.specialEffectTypes[bracelets.specialEffectTypes.IndexOf(bracelets.specialEffectTypes.First(x => x.Name1 == FixedStringValue1))].Name2;
                }
                if (bracelets.specialEffectTypes.Any(x => x.Name1 == FixedStringValue2))
                {
                    value2 = bracelets.specialEffectTypes[bracelets.specialEffectTypes.IndexOf(bracelets.specialEffectTypes.First(x => x.Name1 == FixedStringValue2))].Name2;
                }
            }

            BottomFixedStringValue1 = value1;
            BottomFixedStringValue2 = value2;
            InitBracelets(bracelets, value1);
            InitBracelets(bracelets, value2);

            foreach (var item in pageEffectItems)
            {
                if (item.Lock == true)
                {
                    bracelets.EffectItems.Add(item.EffectItem);
                }
                else
                {
                    bracelets.EffectItems.Add(new EffectItem());
                }
                item.Grade = "";
            }

            for (int i = 0; i < 3; i++)
            {
                while (true)
                {
                    bool temp = false;
                    Random random = new();
                    var randomValue = random.NextDouble();
                    List<double> probability = [0.35, 0.35, 0.3];
                    int index = 0;
                    foreach (var probabilityValue in probability)
                    {
                        if (randomValue < probabilityValue)
                        {
                            if (index == 0)
                            {
                                temp = bracelets.SetItem(LoaBracelets.EBraceletsEffectType.기본효과, IsArtifact && IsUpgrading == false);
                                break;
                            }
                            else if (index == 1)
                            {
                                temp = bracelets.SetItem(LoaBracelets.EBraceletsEffectType.전투특성, IsArtifact && IsUpgrading == false);
                                break;
                            }
                            else
                            {
                                temp = bracelets.SetItem(LoaBracelets.EBraceletsEffectType.특수효과, IsArtifact && IsUpgrading == false);
                                break;
                            }
                        }
                        index++;
                        randomValue -= probabilityValue;
                    }
                    if (temp)
                    {
                        break;
                    }
                }
            }
            int itemIndex = 0;
            foreach (var item in pageEffectItems)
            {
                pageEffectItems[0 + itemIndex].EffectItem = bracelets.EffectItems[2 + itemIndex];
                item.Text = item.EffectItem.EffectTypeEnum.ToString() == "특수효과" ? item.EffectItem.Name : item.EffectItem.EffectTypeEnum.ToString();
                if (item.EffectItem.EffectTypeEnum.ToString() == "특수효과")
                {
                    var temp = bracelets.specialEffectTypes.Where(x => x.Type == item.EffectItem.Type);
                    int index = 0;
                    foreach (var tempItem in temp)
                    {
                        if (IsArtifact && IsUpgrading == false)
                        {
                            if (tempItem.Name1 == item.EffectItem.Name)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (tempItem.Name2 == item.EffectItem.Name)
                            {
                                break;
                            }
                        }
                        index++;
                    }
                    item.Grade = index switch
                    {
                        0 => "#00A4E7",
                        1 => "#9631B7",
                        2 => "#EE8D00",
                        _ => "",
                    };
                }
                itemIndex++;
            }
            if (bSpecialConvertCount)
            {
                SpecialConvertCount++;
            }
            else
            {
                ConvertCount++;
            }
        }

        public (int, int, int, int) Test2(bool bSpecial, bool bTwo, int grantEffectCount, int count, int okCount, List<int> option, List<int> doubleOption, bool bConstans = false)
        {
            if (bTwo)
            {
                FixedStringValue1 = "특화";
                FixedStringValue2 = "치명";
            }
            else
            {
                FixedStringValue1 = "특화";
                FixedStringValue2 = "힘";
            }
            GrantEffect = $"{grantEffectCount}개";
            GrantCount = grantEffectCount;
            int susessCount = 0;
            int doubleCount1 = 0;
            int doubleCount2 = 0;
            int doubleCount3 = 0;

            if (!bSpecial)
            {
                for (int i = 0; i < count; i++)
                {
                    InitializeClick();
                    while (true)
                    {
                        if (ConvertCount == MaxConvertCount)
                        {
                            if (pageEffectItems.Where(x => option.Any(y => y == x.EffectItem.Type)).Count() >= okCount)
                            {
                                if (bTwo == false && !pageEffectItems.Any(x => x.EffectItem.EffectTypeEnum == EEffectType.치명))
                                {
                                    if (bConstans)
                                    {
                                        break;
                                    }
                                }
                                int doubleOptionCount = pageEffectItems.Where(x => doubleOption.Any(y => y == x.EffectItem.Type)).Count();
                                if (doubleOptionCount == 1)
                                {
                                    doubleCount1++;
                                }
                                if (doubleOptionCount == 2)
                                {
                                    doubleCount2++;
                                }
                                if (doubleOptionCount == 3)
                                {
                                    doubleCount3++;
                                }
                                susessCount++;
                                break;
                            }
                            break;
                        }
                        ConvertClick();

                        foreach (var item in pageEffectItems)
                        {
                            if (bTwo == false && item.EffectItem.EffectTypeEnum == EEffectType.치명)
                            {
                                item.Lock = true;
                            }
                            if (item.EffectItem.EffectTypeEnum == EEffectType.특수효과 && option.Any(x => x == item.EffectItem.Type))
                            {
                                item.Lock = true;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    InitializeClick();
                    while (true)
                    {
                        if (ConvertCount == MaxConvertCount && SpecialConvertCount == MaxSpecialConvertCount)
                        {
                            if (pageEffectItems.Where(x => option.Any(y => y == x.EffectItem.Type)).Count() >= okCount)
                            {
                                if (bTwo == false && !pageEffectItems.Any(x => x.EffectItem.EffectTypeEnum == EEffectType.치명))
                                {
                                    if (bConstans)
                                    {
                                        break;
                                    }
                                }
                                int doubleOptionCount = pageEffectItems.Where(x => doubleOption.Any(y => y == x.EffectItem.Type)).Count();
                                if (doubleOptionCount == 1)
                                {
                                    doubleCount1++;
                                }
                                if (doubleOptionCount == 2)
                                {
                                    doubleCount2++;
                                }
                                if (doubleOptionCount == 3)
                                {
                                    doubleCount3++;
                                }
                                susessCount++;
                                break;
                            }
                            break;
                        }
                        ConvertClick(ConvertCount == MaxConvertCount);

                        foreach (var item in pageEffectItems)
                        {
                            if (bTwo == false && item.EffectItem.EffectTypeEnum == EEffectType.치명)
                            {
                                item.Lock = true;
                            }
                            if (item.EffectItem.EffectTypeEnum == EEffectType.특수효과 && option.Any(x => x == item.EffectItem.Type))
                            {
                                item.Lock = true;
                            }
                        }
                    }
                }
            }
            return (susessCount, doubleCount1, doubleCount2, doubleCount3);
        }

        public void Test()
        {
            //List<int> option = [10, 11, 12, 13, 14, 19, 20, 21, 22, 23, 24, 30, 31, 32,];
            //List<int> doubleOption = [10, 11, 12, 13, 14, 19, 20, 21];
            List<int> option = [15, 16, 17, 18, 27, 28, 29];
            List<int> doubleOption = [15, 16, 17, 18,];
            int count = 1000000;
            List<(int, int, int, int)> result =
            [
                Test2(false, true, 3, count, 3, option, doubleOption),
                Test2(true, true, 3, count, 3, option, doubleOption),
                Test2(false, true, 3, count, 2, option, doubleOption),
                Test2(true, true, 3, count, 2, option, doubleOption),
                Test2(false, true, 3, count, 1, option, doubleOption),
                Test2(true, true, 3, count, 1, option, doubleOption),
                Test2(false, true, 2, count, 2, option, doubleOption),
                Test2(true, true, 2, count, 2, option, doubleOption),
                Test2(false, true, 2, count, 1, option, doubleOption),
                Test2(true, true, 2, count, 1, option, doubleOption),
                Test2(false, true, 1, count, 1, option, doubleOption),
                Test2(true, true, 1, count, 1, option, doubleOption),
                Test2(false, false, 3, count, 3, option, doubleOption),
                Test2(true, false, 3, count, 3, option, doubleOption),
                Test2(false, false, 3, count, 2, option, doubleOption),
                Test2(true, false, 3, count, 2, option, doubleOption),
                Test2(false, false, 3, count, 1, option, doubleOption),
                Test2(true, false, 3, count, 1, option, doubleOption),
                Test2(false, false, 2, count, 2, option, doubleOption),
                Test2(true, false, 2, count, 2, option, doubleOption),
                Test2(false, false, 2, count, 1, option, doubleOption),
                Test2(true, false, 2, count, 1, option, doubleOption),
                Test2(false, false, 1, count, 1, option, doubleOption),
                Test2(true, false, 1, count, 1, option, doubleOption),
                Test2(false, false, 3, count, 2, option, doubleOption, true),
                Test2(true, false, 3, count, 2, option, doubleOption, true),
                Test2(false, false, 3, count, 1, option, doubleOption, true),
                Test2(true, false, 3, count, 1, option, doubleOption, true),
                Test2(false, false, 2, count, 1, option, doubleOption, true),
                Test2(true, false, 2, count, 1, option, doubleOption, true),
            ];

            StringBuilder stringBuilder = new();
            foreach (var item in result)
            {
                stringBuilder.AppendLine($"{item.Item1},{item.Item2},{item.Item3},{item.Item4},");
            }
        }
    }
}
