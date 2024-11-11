using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyMoon.Client.Class.Loa
{
    public class LoaBracelets
    {
        public enum EBraceletsEffectType
        {
            None,
            기본효과,
            전투특성,
            특수효과,
        }

        public enum EEffectType
        {
            None,
            힘,
            체력,
            치명,
            특화,
            제압,
            신속,
            인내,
            숙련,
            특수효과,
        }

        public class EffectType
        {
            public EEffectType EffectTypeEnum { get; set; }
            public int Type { get; set; } = 0;
            public string Name1 { get; set; } = string.Empty;
            public string Name2 { get; set; } = string.Empty;
            public double Probability { get; set; }
        }

        public class EffectItem
        {
            public bool IsNull { get; set; } = true;
            public EBraceletsEffectType BraceletsEffectEnum { get; set; }
            public EEffectType EffectTypeEnum { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Type { get; set; } = 0;
            public double Value { get; set; } = 0;
        }

        public List<EffectType> basicEffectTypes =
            [
            new() { EffectTypeEnum = EEffectType.힘, Probability = 0.5 },
            new() { EffectTypeEnum = EEffectType.체력, Probability = 0.5 }
            ];

        public List<EffectType> combatEffectTypes =
            [
            new() { EffectTypeEnum = EEffectType.치명, Probability = 0.1667 },
            new() { EffectTypeEnum = EEffectType.특화, Probability = 0.1667 },
            new() { EffectTypeEnum = EEffectType.제압, Probability = 0.1667 },
            new() { EffectTypeEnum = EEffectType.신속, Probability = 0.1667 },
            new() { EffectTypeEnum = EEffectType.인내, Probability = 0.1667 },
            new() { EffectTypeEnum = EEffectType.숙련, Probability = 0.1667 },
            ];

        public List<EffectType> specialEffectTypes =
            [
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 0, Name1 = @"공격 및 이동 속도가 3% 증가한다.", Name2 = @"공격 및 이동 속도가 4% 증가한다.", Probability = 0.042 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 0, Name1 = @"공격 및 이동 속도가 4% 증가한다.", Name2 = @"공격 및 이동 속도가 5% 증가한다.", Probability = 0.021 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 0, Name1 = @"공격 및 이동 속도가 5% 증가한다.", Name2 = @"공격 및 이동 속도가 6% 증가한다.", Probability = 0.007 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 1, Name1 = @"시드 등급 이하 몬스터에게 주는 피해가 3% 증가한다.", Name2 = @"시드 등급 이하 몬스터에게 주는 피해가 4% 증가한다.", Probability = 0.042 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 1, Name1 = @"시드 등급 이하 몬스터에게 주는 피해가 4% 증가한다.", Name2 = @"시드 등급 이하 몬스터에게 주는 피해가 5% 증가한다.", Probability = 0.021 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 1, Name1 = @"시드 등급 이하 몬스터에게 주는 피해가 5% 증가한다.", Name2 = @"시드 등급 이하 몬스터에게 주는 피해가 6% 증가한다.", Probability = 0.007 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 2, Name1 = @"시드 등급 이하 몬스터에게 받는 피해가 4% 감소한다.", Name2 = @"시드 등급 이하 몬스터에게 받는 피해가 6% 감소한다.", Probability = 0.042 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 2, Name1 = @"시드 등급 이하 몬스터에게 받는 피해가 6% 감소한다.", Name2 = @"시드 등급 이하 몬스터에게 받는 피해가 8% 감소한다.", Probability = 0.021 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 2, Name1 = @"시드 등급 이하 몬스터에게 받는 피해가 8% 감소한다.", Name2 = @"시드 등급 이하 몬스터에게 받는 피해가 10% 감소한다.", Probability = 0.007 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 3, Name1 = @"물리 방어력 +4000", Name2 = @"물리 방어력 +5000", Probability = 0.042 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 3, Name1 = @"물리 방어력 +5000", Name2 = @"물리 방어력 +6000", Probability = 0.021 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 3, Name1 = @"물리 방어력 +6000", Name2 = @"물리 방어력 +7000", Probability = 0.007 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 4, Name1 = @"마법 방어력 +4000", Name2 = @"마법 방어력 +5000", Probability = 0.042 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 4, Name1 = @"마법 방어력 +5000", Name2 = @"마법 방어력 +6000", Probability = 0.021 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 4, Name1 = @"마법 방어력 +6000", Name2 = @"마법 방어력 +7000", Probability = 0.007 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 5, Name1 = @"최대 생명력 +8400", Name2 = @"최대 생명력 +11200", Probability = 0.042 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 5, Name1 = @"최대 생명력 +11200", Name2 = @"최대 생명력 +14000", Probability = 0.021 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 5, Name1 = @"최대 생명력 +14000", Name2 = @"최대 생명력 +16800", Probability = 0.007 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 6, Name1 = @"전투 중 생명력 회복량 +80", Name2 = @"전투 중 생명력 회복량 +100", Probability = 0.042 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 6, Name1 = @"전투 중 생명력 회복량 +100", Name2 = @"전투 중 생명력 회복량 +130", Probability = 0.021 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 6, Name1 = @"전투 중 생명력 회복량 +130", Name2 = @"전투 중 생명력 회복량 +160", Probability = 0.007 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 7, Name1 = @"전투자원 자연 회복량 +6%", Name2 = @"전투자원 자연 회복량 +8%", Probability = 0.042 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 7, Name1 = @"전투자원 자연 회복량 +8%", Name2 = @"전투자원 자연 회복량 +10%", Probability = 0.021 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 7, Name1 = @"전투자원 자연 회복량 +10%", Name2 = @"전투자원 자연 회복량 +12%", Probability = 0.007 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 8, Name1 = @"이동기 및 기상기 재사용 대기 시간이 6% 감소한다.", Name2 = @"이동기 및 기상기 재사용 대기 시간이 8% 감소한다.", Probability = 0.042 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 8, Name1 = @"이동기 및 기상기 재사용 대기 시간이 8% 감소한다.", Name2 = @"이동기 및 기상기 재사용 대기 시간이 10% 감소한다.", Probability = 0.021 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 8, Name1 = @"이동기 및 기상기 재사용 대기 시간이 10% 감소한다.", Name2 = @"이동기 및 기상기 재사용 대기 시간이 12% 감소한다.", Probability = 0.007 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 9, Name1 = @"공격 적중 시 90초 동안 경직 및 피격 이상에 면역이 된다. (재사용 대기 시간 90초) 해당 효과는 1회 피격 시 사라진다.", Name2 = @"공격 적중 시 80초 동안 경직 및 피격 이상에 면역이 된다. (재사용 대기 시간 80초) 해당 효과는 1회 피격 시 사라진다.", Probability = 0.042 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 9, Name1 = @"공격 적중 시 80초 동안 경직 및 피격 이상에 면역이 된다. (재사용 대기 시간 80초) 해당 효과는 1회 피격 시 사라진다.", Name2 = @"공격 적중 시 70초 동안 경직 및 피격 이상에 면역이 된다. (재사용 대기 시간 70초) 해당 효과는 1회 피격 시 사라진다.", Probability = 0.021 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 9, Name1 = @"공격 적중 시 70초 동안 경직 및 피격 이상에 면역이 된다. (재사용 대기 시간 70초) 해당 효과는 1회 피격 시 사라진다.", Name2 = @"공격 적중 시 60초 동안 경직 및 피격 이상에 면역이 된다. (재사용 대기 시간 60초) 해당 효과는 1회 피격 시 사라진다.", Probability = 0.007 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 10, Name1 = @"치명타 적중률이 2.6% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Name2 = @"치명타 적중률이 3.4% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 10, Name1 = @"치명타 적중률이 3.4% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Name2 = @"치명타 적중률이 4.2% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 10, Name1 = @"치명타 적중률이 4.2% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Name2 = @"치명타 적중률이 5.0% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 11, Name1 = @"치명타 피해가 5.2% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Name2 = @"치명타 피해가 6.8% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 11, Name1 = @"치명타 피해가 6.8% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Name2 = @"치명타 피해가 8.4% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 11, Name1 = @"치명타 피해가 8.4% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Name2 = @"치명타 피해가 10.0% 증가한다. 공격이 치명타로 적중 시 적에게 주는 피해가 1.5% 증가한다.", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 12, Name1 = @"적에게 주는 피해가 1.5% 증가하며, 무력화 상태의 적에게 주는 피해가 3.5% 증가한다.", Name2 = @"적에게 주는 피해가 2.0% 증가하며, 무력화 상태의 적에게 주는 피해가 4.0% 증가한다.", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 12, Name1 = @"적에게 주는 피해가 2.0% 증가하며, 무력화 상태의 적에게 주는 피해가 4.0% 증가한다.", Name2 = @"적에게 주는 피해가 2.5% 증가하며, 무력화 상태의 적에게 주는 피해가 4.5% 증가한다.", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 12, Name1 = @"적에게 주는 피해가 2.5% 증가하며, 무력화 상태의 적에게 주는 피해가 4.5% 증가한다.", Name2 = @"적에게 주는 피해가 3.0% 증가하며, 무력화 상태의 적에게 주는 피해가 5.0% 증가한다.", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 13, Name1 = @"추가 피해가 2.0% 증가한다. 악마 및 대악마 계열 피해량이 2.5% 증가한다.", Name2 = @"추가 피해가 2.5% 증가한다. 악마 및 대악마 계열 피해량이 2.5% 증가한다.", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 13, Name1 = @"추가 피해가 2.5% 증가한다. 악마 및 대악마 계열 피해량이 2.5% 증가한다.", Name2 = @"추가 피해가 3.0% 증가한다. 악마 및 대악마 계열 피해량이 2.5% 증가한다.", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 13, Name1 = @"추가 피해가 3.0% 증가한다. 악마 및 대악마 계열 피해량이 2.5% 증가한다.", Name2 = @"추가 피해가 3.5% 증가한다. 악마 및 대악마 계열 피해량이 2.5% 증가한다.", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 14, Name1 = @"스킬의 재사용 대기 시간이 2% 증가하지만, 적에게 주는 피해가 4.0% 증가한다.", Name2 = @"스킬의 재사용 대기 시간이 2% 증가하지만, 적에게 주는 피해가 4.5% 증가한다.", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 14, Name1 = @"스킬의 재사용 대기 시간이 2% 증가하지만, 적에게 주는 피해가 4.5% 증가한다.", Name2 = @"스킬의 재사용 대기 시간이 2% 증가하지만, 적에게 주는 피해가 5.0% 증가한다.", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 14, Name1 = @"스킬의 재사용 대기 시간이 2% 증가하지만, 적에게 주는 피해가 5.0% 증가한다.", Name2 = @"스킬의 재사용 대기 시간이 2% 증가하지만, 적에게 주는 피해가 5.5% 증가한다.", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 15, Name1 = @"적에게 공격 적중 시 8초 동안 대상의 방어력을 1.5% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 1.5% 증가한다.", Name2 = @"적에게 공격 적중 시 8초 동안 대상의 방어력을 1.8% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.0% 증가한다.", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 15, Name1 = @"적에게 공격 적중 시 8초 동안 대상의 방어력을 1.8% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.0% 증가한다.", Name2 = @"적에게 공격 적중 시 8초 동안 대상의 방어력을 2.1% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.5% 증가한다.", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 15, Name1 = @"적에게 공격 적중 시 8초 동안 대상의 방어력을 2.1% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.5% 증가한다.", Name2 = @"적에게 공격 적중 시 8초 동안 대상의 방어력을 2.5% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 3.0% 증가한다.", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 16, Name1 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 저항을 1.5% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 1.5% 증가한다.", Name2 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 저항을 1.8% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.0% 증가한다.", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 16, Name1 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 저항을 1.8% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.0% 증가한다.", Name2 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 저항을 2.1% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.5% 증가한다.", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 16, Name1 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 저항을 2.1% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.5% 증가한다.", Name2 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 저항을 2.5% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 3.0% 증가한다.", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 17, Name1 = @"파티 효과로 보호 효과가 적용된 대상이 5초 동안 적에게 주는 피해가 0.7% 증가한다. 해당 효과는 한 파티 당 하나만 적용되며, 지속 시간이 없는 보호 효과에는 적용되지 않는다. 아군 공격력 강화 효과가 1.5% 증가한다.", Name2 = @"파티 효과로 보호 효과가 적용된 대상이 5초 동안 적에게 주는 피해가 0.9% 증가한다. 해당 효과는 한 파티 당 하나만 적용되며, 지속 시간이 없는 보호 효과에는 적용되지 않는다. 아군 공격력 강화 효과가 2.0% 증가한다.", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 17, Name1 = @"파티 효과로 보호 효과가 적용된 대상이 5초 동안 적에게 주는 피해가 0.9% 증가한다. 해당 효과는 한 파티 당 하나만 적용되며, 지속 시간이 없는 보호 효과에는 적용되지 않는다. 아군 공격력 강화 효과가 2.0% 증가한다.", Name2 = @"파티 효과로 보호 효과가 적용된 대상이 5초 동안 적에게 주는 피해가 1.1% 증가한다. 해당 효과는 한 파티 당 하나만 적용되며, 지속 시간이 없는 보호 효과에는 적용되지 않는다. 아군 공격력 강화 효과가 2.5% 증가한다.", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 17, Name1 = @"파티 효과로 보호 효과가 적용된 대상이 5초 동안 적에게 주는 피해가 1.1% 증가한다. 해당 효과는 한 파티 당 하나만 적용되며, 지속 시간이 없는 보호 효과에는 적용되지 않는다. 아군 공격력 강화 효과가 2.5% 증가한다.", Name2 = @"파티 효과로 보호 효과가 적용된 대상이 5초 동안 적에게 주는 피해가 1.3% 증가한다. 해당 효과는 한 파티 당 하나만 적용되며, 지속 시간이 없는 보호 효과에는 적용되지 않는다. 아군 공격력 강화 효과가 3.0% 증가한다.", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 18, Name1 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 피해 저항을 3.0% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 1.5% 증가한다.", Name2 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 피해 저항을 3.6% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.0% 증가한다.", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 18, Name1 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 피해 저항을 3.6% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.0% 증가한다.", Name2 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 피해 저항을 4.2% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.5% 증가한다.", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 18, Name1 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 피해 저항을 4.2% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 2.5% 증가한다.", Name2 = @"적에게 공격 적중 시 8초 동안 대상의 치명타 피해 저항을 4.8% 감소시킨다. 해당 효과는 한 파티 당 하나만 적용된다. 아군 공격력 강화 효과가 3.0% 증가한다.", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 19, Name1 = @"공격 적중 시 매 초 마다 10초 동안 무기 공격력이 1000, 공격 및 이동 속도가 1% 증가한다.(최대 6중첩)", Name2 = @"공격 적중 시 매 초 마다 10초 동안 무기 공격력이 1160, 공격 및 이동 속도가 1% 증가한다.(최대 6중첩)", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 19, Name1 = @"공격 적중 시 매 초 마다 10초 동안 무기 공격력이 1160, 공격 및 이동 속도가 1% 증가한다.(최대 6중첩)", Name2 = @"공격 적중 시 매 초 마다 10초 동안 무기 공격력이 1320, 공격 및 이동 속도가 1% 증가한다.(최대 6중첩)", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 19, Name1 = @"공격 적중 시 매 초 마다 10초 동안 무기 공격력이 1320, 공격 및 이동 속도가 1% 증가한다.(최대 6중첩)", Name2 = @"공격 적중 시 매 초 마다 10초 동안 무기 공격력이 1480, 공격 및 이동 속도가 1% 증가한다.(최대 6중첩)", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 20, Name1 = @"무기 공격력이 6300 증가한다. 자신의 생명력이 50% 이상일 경우 적에게 공격 적중 시 5초 동안 무기 공격력이 1800 증가한다.", Name2 = @"무기 공격력이 7200 증가한다. 자신의 생명력이 50% 이상일 경우 적에게 공격 적중 시 5초 동안 무기 공격력이 2000 증가한다.", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 20, Name1 = @"무기 공격력이 7200 증가한다. 자신의 생명력이 50% 이상일 경우 적에게 공격 적중 시 5초 동안 무기 공격력이 2000 증가한다.", Name2 = @"무기 공격력이 8100 증가한다. 자신의 생명력이 50% 이상일 경우 적에게 공격 적중 시 5초 동안 무기 공격력이 2200 증가한다.", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 20, Name1 = @"무기 공격력이 8100 증가한다. 자신의 생명력이 50% 이상일 경우 적에게 공격 적중 시 5초 동안 무기 공격력이 2200 증가한다.", Name2 = @"무기 공격력이 9000 증가한다. 자신의 생명력이 50% 이상일 경우 적에게 공격 적중 시 5초 동안 무기 공격력이 2400 증가한다.", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 21, Name1 = @"무기 공격력이 6000 증가한다. 공격 적중 시 30초 마다 120초 동안 무기 공격력이 120 증가한다. (최대 30중첩)", Name2 = @"무기 공격력이 6900 증가한다. 공격 적중 시 30초 마다 120초 동안 무기 공격력이 130 증가한다. (최대 30중첩)", Probability = 0.005 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 21, Name1 = @"무기 공격력이 6900 증가한다. 공격 적중 시 30초 마다 120초 동안 무기 공격력이 130 증가한다. (최대 30중첩)", Name2 = @"무기 공격력이 7800 증가한다. 공격 적중 시 30초 마다 120초 동안 무기 공격력이 140 증가한다. (최대 30중첩)", Probability = 0.0025 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 21, Name1 = @"무기 공격력이 7800 증가한다. 공격 적중 시 30초 마다 120초 동안 무기 공격력이 140 증가한다. (최대 30중첩)", Name2 = @"무기 공격력이 8700 증가한다. 공격 적중 시 30초 마다 120초 동안 무기 공격력이 150 증가한다. (최대 30중첩)", Probability = 0.0008333 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 22, Name1 = @"적에게 주는 피해가 1.5% 증가한다.", Name2 = @"적에게 주는 피해가 2.0% 증가한다.", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 22, Name1 = @"적에게 주는 피해가 2.0% 증가한다.", Name2 = @"적에게 주는 피해가 2.5% 증가한다.", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 22, Name1 = @"적에게 주는 피해가 2.5% 증가한다.", Name2 = @"적에게 주는 피해가 3.0% 증가한다.", Probability = 0.001818 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 23, Name1 = @"추가 피해 +2.5%", Name2 = @"추가 피해 +3.0%", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 23, Name1 = @"추가 피해 +3.0%", Name2 = @"추가 피해 +3.5%", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 23, Name1 = @"추가 피해 +3.5%", Name2 = @"추가 피해 +4.0%", Probability = 0.001818 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 24, Name1 = @"백어택 스킬이 적에게 주는 피해가 2.0% 증가한다.", Name2 = @"백어택 스킬이 적에게 주는 피해가 2.5% 증가한다.", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 24, Name1 = @"백어택 스킬이 적에게 주는 피해가 2.5% 증가한다.", Name2 = @"백어택 스킬이 적에게 주는 피해가 3.0% 증가한다.", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 24, Name1 = @"백어택 스킬이 적에게 주는 피해가 3.0% 증가한다.", Name2 = @"백어택 스킬이 적에게 주는 피해가 3.5% 증가한다.", Probability = 0.001818 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 25, Name1 = @"헤드어택 스킬이 적에게 주는 피해가 2.0% 증가한다.", Name2 = @"헤드어택 스킬이 적에게 주는 피해가 2.5% 증가한다.", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 25, Name1 = @"헤드어택 스킬이 적에게 주는 피해가 2.5% 증가한다.", Name2 = @"헤드어택 스킬이 적에게 주는 피해가 3.0% 증가한다.", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 25, Name1 = @"헤드어택 스킬이 적에게 주는 피해가 3.0% 증가한다.", Name2 = @"헤드어택 스킬이 적에게 주는 피해가 3.5% 증가한다.", Probability = 0.001818 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 26, Name1 = @"방향성 공격이 아닌 스킬이 적에게 주는 피해가 2.0% 증가한다. 각성기는 적용되지 않는다.", Name2 = @"방향성 공격이 아닌 스킬이 적에게 주는 피해가 2.5% 증가한다. 각성기는 적용되지 않는다.", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 26, Name1 = @"방향성 공격이 아닌 스킬이 적에게 주는 피해가 2.5% 증가한다. 각성기는 적용되지 않는다.", Name2 = @"방향성 공격이 아닌 스킬이 적에게 주는 피해가 3.0% 증가한다. 각성기는 적용되지 않는다.", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 26, Name1 = @"방향성 공격이 아닌 스킬이 적에게 주는 피해가 3.0% 증가한다. 각성기는 적용되지 않는다.", Name2 = @"방향성 공격이 아닌 스킬이 적에게 주는 피해가 3.5% 증가한다. 각성기는 적용되지 않는다.", Probability = 0.001818 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 27, Name1 = @"파티원 보호 및 회복 효과가 2.0% 증가한다.", Name2 = @"파티원 보호 및 회복 효과가 2.5% 증가한다.", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 27, Name1 = @"파티원 보호 및 회복 효과가 2.5% 증가한다.", Name2 = @"파티원 보호 및 회복 효과가 3.0% 증가한다.", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 27, Name1 = @"파티원 보호 및 회복 효과가 3.0% 증가한다.", Name2 = @"파티원 보호 및 회복 효과가 3.5% 증가한다.", Probability = 0.001818 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 28, Name1 = @"아군 공격력 강화 효과 +3.0%", Name2 = @"아군 공격력 강화 효과 +4.0%", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 28, Name1 = @"아군 공격력 강화 효과 +4.0%", Name2 = @"아군 공격력 강화 효과 +5.0%", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 28, Name1 = @"아군 공격력 강화 효과 +5.0%", Name2 = @"아군 공격력 강화 효과 +6.0%", Probability = 0.001818 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 29, Name1 = @"아군 피해량 강화 효과 +4.5%", Name2 = @"아군 피해량 강화 효과 +6.0%", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 29, Name1 = @"아군 피해량 강화 효과 +6.0%", Name2 = @"아군 피해량 강화 효과 +7.5%", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 29, Name1 = @"아군 피해량 강화 효과 +7.5%", Name2 = @"아군 피해량 강화 효과 +9.0%", Probability = 0.001818 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 30, Name1 = @"치명타 적중률 +2.6%", Name2 = @"치명타 적중률 +3.4%", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 30, Name1 = @"치명타 적중률 +3.4%", Name2 = @"치명타 적중률 +4.2%", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 30, Name1 = @"치명타 적중률 +4.2%", Name2 = @"치명타 적중률 +5.0%", Probability = 0.001818 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 31, Name1 = @"치명타 피해 +5.2%", Name2 = @"치명타 피해 +6.8%", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 31, Name1 = @"치명타 피해 +6.8%", Name2 = @"치명타 피해 +8.4%", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 31, Name1 = @"치명타 피해 +8.4%", Name2 = @"치명타 피해 +10.0%", Probability = 0.001818 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 32, Name1 = @"무기 공격력 +6300", Name2 = @"무기 공격력 +7200", Probability = 0.010909 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 32, Name1 = @"무기 공격력 +7200", Name2 = @"무기 공격력 +8100", Probability = 0.005455 },
            new() { EffectTypeEnum = EEffectType.특수효과, Type = 32, Name1 = @"무기 공격력 +8100", Name2 = @"무기 공격력 +9000", Probability = 0.001818 },
            ];

        public List<EffectItem> EffectItems { get; set; } = [];

        public const int BASIC_EFFECT_MAX_COUNT = 2;
        public const int COMBAT_EFFECT_MAX_COUNT = 2;

        public bool SetItem(EBraceletsEffectType eBraceletsEffectType, bool bArtifact)
        {
            switch (eBraceletsEffectType)
            {
                case EBraceletsEffectType.None:
                    break;
                case EBraceletsEffectType.기본효과:
                    return SetItem(basicEffectTypes, eBraceletsEffectType, bArtifact);
                case EBraceletsEffectType.전투특성:
                    return SetItem(combatEffectTypes, eBraceletsEffectType, bArtifact);
                case EBraceletsEffectType.특수효과:
                    return SetItem(specialEffectTypes, eBraceletsEffectType, bArtifact);
                default:
                    break;
            }
            return false;
        }

        private bool SetItem(List<EffectType> effectTypes, EBraceletsEffectType eBraceletsEffectType, bool bArtifact)
        {
            if (eBraceletsEffectType == EBraceletsEffectType.기본효과)
            {
                if (EffectItems.Where(x => x.BraceletsEffectEnum == EBraceletsEffectType.기본효과).Count() >= BASIC_EFFECT_MAX_COUNT)
                {
                    return false;
                }
            }
            if (eBraceletsEffectType == EBraceletsEffectType.전투특성)
            {
                if (EffectItems.Where(x => x.BraceletsEffectEnum == EBraceletsEffectType.전투특성).Count() >= COMBAT_EFFECT_MAX_COUNT)
                {
                    return false;
                }
            }
            while (true)
            {
                bool doAdd = false;
                Random random = new();
                var randomValue = random.NextDouble();
                foreach (EffectType item in effectTypes)
                {
                    if (randomValue < item.Probability)
                    {
                        if (item.EffectTypeEnum == EEffectType.특수효과)
                        {
                            if (EffectItems.Any(x => x.Name == item.Name1 || x.Name == item.Name2 || x.Type == item.Type) == false)
                            {
                                doAdd = true;
                                foreach (var effectItem in EffectItems)
                                {
                                    if (effectItem.IsNull == true)
                                    {
                                        effectItem.EffectTypeEnum = item.EffectTypeEnum;
                                        effectItem.BraceletsEffectEnum = eBraceletsEffectType;
                                        effectItem.Name = bArtifact ? item.Name1 : item.Name2;
                                        effectItem.Type = item.Type;
                                        effectItem.IsNull = false;
                                        break;
                                    }
                                }
                                //Todo
                            }
                        }
                        else if (EffectItems.Any(x => x.EffectTypeEnum == item.EffectTypeEnum) == false)
                        {
                            doAdd = true;
                            foreach (var effectItem in EffectItems)
                            {
                                if (effectItem.IsNull == true)
                                {
                                    effectItem.EffectTypeEnum = item.EffectTypeEnum;
                                    effectItem.BraceletsEffectEnum = eBraceletsEffectType;
                                    effectItem.IsNull = false;
                                    break;
                                }
                            }
                            //Todo
                        }
                        break;
                    }
                    randomValue -= item.Probability;
                }
                if (doAdd)
                {
                    break;
                }
            }
            return true;
        }
    }
}
